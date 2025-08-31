using System;
using UnityEngine;
using VContainer.Unity;
//实现ITimeService接口，如果别的组件想创建这个接口实例，会由GameInstaller告诉别的组件获得了这个接口，但这个接口是由RealTimeService提供的
//别的组件获得了ITimeService后，将方法添加到了RealTImeService的OnTimeUpdate事件中，此时还不会执行，因为没有执行调用，
//实现ITickable告诉这个组件说我想要每帧执行Tick这个函数，在Tick这个函数中就可以调用事件了；
//现在需要到GameInstaller中注册实现了者两个接口的类，那么就可以正常添加事件和每帧执行Tick这个函数了
public class RealTimeService : ITimeService, ITickable
{
    private float _currentTime = 0f;
    //返回当前的时间
    public float CurrentTime => _currentTime;

    public event Action<float> OnTImeUpdate;

    public void Tick()
    {
        float deltaTime = Time.deltaTime;
        _currentTime += deltaTime;
        //最终订阅的事件都会在这里执行
        OnTImeUpdate?.Invoke(_currentTime);
    }
}
