using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameInstaller : LifetimeScope
{
    //��Ҫ��auto inject GameObject�з���ʵ��ע��ӿ���������صĶ�����SkyManager
    protected override void Configure(IContainerBuilder builder)
    {
        //��������� VContainer����������Ҫ ITimeService ʱ���ʹ���һ�� RealTimeService
        builder.Register<RealTimeService>(Lifetime.Singleton)
               .As<ITimeService>()
               .As<ITickable>();
    }
}
