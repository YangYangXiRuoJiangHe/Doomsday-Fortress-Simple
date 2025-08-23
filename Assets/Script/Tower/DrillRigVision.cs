using System.Collections;
using UnityEngine;

public class DrillRigVision : MonoBehaviour
{
    public Rotate drillWheel;
    //钻头
    public Transform drillBit;
    public Transform holder;
    //钻的长度
    public float drillOffsety;
    public float drillDuration;
    public float reloadDuration;

    public Transform sideWire;
    public Transform sideHandle;
    private void Start()
    {
        InvokeRepeating(nameof(StartDrillIron), .5f, drillDuration + reloadDuration + 1f);
    }
    public void StartDrillIron()
    {
        StopAllCoroutines();
        StartCoroutine(DrillBitAttackCo());
    }

    private IEnumerator DrillBitAttackCo()
    {
        drillWheel.AdjustRotationSpeed(25);
        StartCoroutine(ChangePositionCo(drillBit, -drillOffsety, drillDuration));
        StartCoroutine(ChangeScaleCo(holder, 7, drillDuration));
        StartCoroutine(ChangePositionCo(sideHandle, .45f, drillDuration));
        StartCoroutine(ChangeScaleCo(sideWire, .1f, drillDuration));
        yield return new WaitForSeconds(drillDuration);
        drillWheel.AdjustRotationSpeed(3);
        StartCoroutine(ChangePositionCo(drillBit, drillOffsety, reloadDuration));
        StartCoroutine(ChangeScaleCo(holder, 1, reloadDuration));
        StartCoroutine(ChangePositionCo(sideHandle, -.45f, reloadDuration));
        StartCoroutine(ChangeScaleCo(sideWire, 1f, reloadDuration));
    }
    private IEnumerator ChangePositionCo(Transform transform,float yoffset,float duration = .1f)
    {
        float timer = 0;
        Vector3 initialPosition = transform.localPosition;
        Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y + yoffset, initialPosition.z);
        while(timer < duration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
    }

    private IEnumerator ChangeScaleCo(Transform transform,float newScale,float duration = .1f)
    {
        float timer = 0;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(initialScale.x, newScale, initialScale.z);

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
