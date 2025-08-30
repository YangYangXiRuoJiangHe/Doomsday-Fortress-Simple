using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// 用于存储每个时间点（阶段）的天空盒参数
[Serializable]
public struct SkyboxStage
{
    public string stageName;
    public Color tintColor;
    public float exposure;
    public float rotation;
}

public class SkyManager : MonoBehaviour
{
    [Header("太阳的组件")]
    public Transform sunTransform;
    [Header("天空盒材质")]
    //不能直接修改的材质
    [SerializeField] private Material originalSkyboxMaterial;
    [Header("时间阶段设置")]
    public SkyboxStage morning;   
    public SkyboxStage afternoon;      
    public SkyboxStage evening;  
    public SkyboxStage night;      
    [Header("当前时间")]
    [Range(0f, 24f)]
    public float currentTime = 12.0f;
    //用于修改的材质
    private Material skyboxInstance;
    // 存储四个阶段的时间点，用于查找正确的插值区间
    private float[] stageTimes;
    private SkyboxStage[] stages;
    private void Awake()
    {
        if (originalSkyboxMaterial == null)
        {
            Debug.LogError("请分配原始天空盒材质！", this);
            enabled = false; // 停用脚本
            return;
        }
        skyboxInstance = new Material(originalSkyboxMaterial);
        RenderSettings.skybox = skyboxInstance;
        stages = new SkyboxStage[] { morning, afternoon, evening, night };
        stageTimes = new float[] {
            0, 
            6,
            12,
            18 
        };
    }
    private void Update()
    {
        //传入的时间只能是0-24
        UpdateSkyboxBasedOnTime(Time.time/60  % 24);
    }

    /// <summary>
    /// 根据传入的时间更新天空盒参数
    /// </summary>
    /// <param name="time">当前时间 (0-24)</param>
    public void UpdateSkyboxBasedOnTime(float time)
    {
        if (skyboxInstance == null)
            return;
        // 查找当前时间位于哪两个阶段之间
        int startIndex = 0;
        int endIndex = 0;
        float t = 0f; // 插值系数

        for(int i = 0; i < stageTimes.Length; i++)
        {
            int nithtToMorning = i +1;
            if (nithtToMorning >= stageTimes.Length)
            {
                startIndex = i;
                endIndex = 0;
                t = (time - stageTimes[startIndex]) / (24 - stageTimes[startIndex]);
                break;
            }
            else if(time >= stageTimes[i] && time < stageTimes[i + 1])
            {
                startIndex = i;
                endIndex = i + 1;
                t = (time - stageTimes[startIndex]) / (stageTimes[endIndex] - stageTimes[startIndex]);
                break;
            }
        }

        SkyboxStage startStage = stages[startIndex];
        SkyboxStage endStage = stages[endIndex];

        //线性插值
        Color lerpedColor = Color.Lerp(startStage.tintColor, endStage.tintColor, t);
        float lerpedExposure = Mathf.Lerp(startStage.exposure, endStage.exposure, t);
        float lerpedRotation = Mathf.LerpAngle(startStage.rotation, endStage.rotation, t);
        // 应用到材质
        //改变颜色
        if (skyboxInstance.HasColor("_Tint"))
            skyboxInstance.SetColor("_Tint", lerpedColor);
        //改变曝光
        if (skyboxInstance.HasFloat("_Exposure"))
            skyboxInstance.SetFloat("_Exposure", lerpedExposure);
        //改变旋转
        if (skyboxInstance.HasFloat("_Rotation"))
            skyboxInstance.SetFloat("_Rotation", lerpedRotation);
        //这是场景中的实时光源太阳的旋转
        float sunRotationX = 270 - lerpedRotation;
        RotateSun(new Vector3(270 - lerpedRotation, sunTransform.rotation.y, sunTransform.rotation.z));

        EnvironmentLightIntensity(lerpedExposure);

    }
    public void EnvironmentLightIntensity(float intensity)
    {
        RenderSettings.ambientIntensity = intensity;
    }
    public void RotateSun(Vector3 rotate)
    {
        sunTransform.rotation = Quaternion.Euler(rotate);
    }
}
