using System;

public interface ITimeService
{
    //����һ������
    float CurrentTime { get; }

    event Action<float> OnTImeUpdate;
}
