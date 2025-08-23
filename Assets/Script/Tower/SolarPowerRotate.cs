using UnityEngine;

public class SolarPowerRotate : Move
{
    public bool canRepeatingRotate;
    protected override void Update()
    {
        base.Update();
        if (transform.eulerAngles.z < 315 && transform.eulerAngles.z > 200)
        {
            canRepeatingRotate = false;
        }
        if (transform.eulerAngles.z > 45 && transform.eulerAngles.z < 200)
        {
            canRepeatingRotate = true;
        }
    }
    public override void TransformRotate()
    {
        if (canRepeatingRotate)
        {
            transform.Rotate(-Vector3.forward * rotateSpeed * Time.deltaTime);

        }
        else
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

        }
    }
}
