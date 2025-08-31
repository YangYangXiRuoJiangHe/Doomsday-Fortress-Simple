using System;

public interface ITimeService
{
    //这是一个方法
    float CurrentTime { get; }

    event Action<float> OnTImeUpdate;
}
