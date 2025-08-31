using System;
using UnityEngine;
using VContainer.Unity;
//ʵ��ITimeService�ӿڣ�����������봴������ӿ�ʵ��������GameInstaller���߱��������������ӿڣ�������ӿ�����RealTimeService�ṩ��
//�����������ITimeService�󣬽�������ӵ���RealTImeService��OnTimeUpdate�¼��У���ʱ������ִ�У���Ϊû��ִ�е��ã�
//ʵ��ITickable����������˵����Ҫÿִ֡��Tick�����������Tick��������оͿ��Ե����¼��ˣ�
//������Ҫ��GameInstaller��ע��ʵ�����������ӿڵ��࣬��ô�Ϳ�����������¼���ÿִ֡��Tick���������
public class RealTimeService : ITimeService, ITickable
{
    private float _currentTime = 0f;
    //���ص�ǰ��ʱ��
    public float CurrentTime => _currentTime;

    public event Action<float> OnTImeUpdate;

    public void Tick()
    {
        float deltaTime = Time.deltaTime;
        _currentTime += deltaTime;
        //���ն��ĵ��¼�����������ִ��
        OnTImeUpdate?.Invoke(_currentTime);
    }
}
