using UnityEngine;

public class SolarPowerRotate : TowerVision
{
    public bool canRepeatingRotate;
    public float rotateSpeed;
    public void Update()
    {
        TransformRotate();
    }
    public void TransformRotate()
    {
        if (transform.eulerAngles.z < 315 && transform.eulerAngles.z > 200)
        {
            canRepeatingRotate = false;
        }
        if (transform.eulerAngles.z > 45 && transform.eulerAngles.z < 200)
        {
            canRepeatingRotate = true;
        }
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
