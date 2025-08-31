using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameInstaller : LifetimeScope
{
    //需要在auto inject GameObject中放入实例注入接口组件所挂载的对象。如SkyManager
    protected override void Configure(IContainerBuilder builder)
    {
        //在这里告诉 VContainer：当有人需要 ITimeService 时，就创建一个 RealTimeService
        builder.Register<RealTimeService>(Lifetime.Singleton)
               .As<ITimeService>()
               .As<ITickable>();
    }
}
