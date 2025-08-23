using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotateVector;
    public float speed = 25.0f;    
    void Update()
    {
        float newRotateSpeed = speed * 100;
        transform.Rotate(rotateVector * newRotateSpeed * Time.deltaTime);
    }
    public void AdjustRotationSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
