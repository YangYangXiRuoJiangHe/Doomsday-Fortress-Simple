using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manage : MonoBehaviour
{
    public static Scene_Manage instance;
    public static bool isStartGame;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void LoadScene(int index)
    {
        if(index >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("�����������ƣ������Ƿ��г���ĳ���");
        }
        SceneManager.LoadScene(index);
    }

}
