using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// ���ڴ洢ÿ��ʱ��㣨�׶Σ�����պв���
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
    [Header("̫�������")]
    public Transform sunTransform;
    [Header("��պв���")]
    //����ֱ���޸ĵĲ���
    [SerializeField] private Material originalSkyboxMaterial;
    [Header("ʱ��׶�����")]
    public SkyboxStage morning;   
    public SkyboxStage afternoon;      
    public SkyboxStage evening;  
    public SkyboxStage night;      
    [Header("��ǰʱ��")]
    [Range(0f, 24f)]
    public float currentTime = 12.0f;
    //�����޸ĵĲ���
    private Material skyboxInstance;
    // �洢�ĸ��׶ε�ʱ��㣬���ڲ�����ȷ�Ĳ�ֵ����
    private float[] stageTimes;
    private SkyboxStage[] stages;
    private void Awake()
    {
        if (originalSkyboxMaterial == null)
        {
            Debug.LogError("�����ԭʼ��պв��ʣ�", this);
            enabled = false; // ͣ�ýű�
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
        //�����ʱ��ֻ����0-24
        UpdateSkyboxBasedOnTime(Time.time/60  % 24);
    }

    /// <summary>
    /// ���ݴ����ʱ�������պв���
    /// </summary>
    /// <param name="time">��ǰʱ�� (0-24)</param>
    public void UpdateSkyboxBasedOnTime(float time)
    {
        if (skyboxInstance == null)
            return;
        // ���ҵ�ǰʱ��λ���������׶�֮��
        int startIndex = 0;
        int endIndex = 0;
        float t = 0f; // ��ֵϵ��

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

        //���Բ�ֵ
        Color lerpedColor = Color.Lerp(startStage.tintColor, endStage.tintColor, t);
        float lerpedExposure = Mathf.Lerp(startStage.exposure, endStage.exposure, t);
        float lerpedRotation = Mathf.LerpAngle(startStage.rotation, endStage.rotation, t);
        // Ӧ�õ�����
        //�ı���ɫ
        if (skyboxInstance.HasColor("_Tint"))
            skyboxInstance.SetColor("_Tint", lerpedColor);
        //�ı��ع�
        if (skyboxInstance.HasFloat("_Exposure"))
            skyboxInstance.SetFloat("_Exposure", lerpedExposure);
        //�ı���ת
        if (skyboxInstance.HasFloat("_Rotation"))
            skyboxInstance.SetFloat("_Rotation", lerpedRotation);
        //���ǳ����е�ʵʱ��Դ̫������ת
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
