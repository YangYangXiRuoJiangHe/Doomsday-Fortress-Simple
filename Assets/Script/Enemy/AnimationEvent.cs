using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public Transform EnemyBody;
    public float sinkDuration = 1.0f; // œ¬≥¡ ±º‰£®√Î£©

    public void StartDeadToGround()
    {
        StartCoroutine(DeadToGround());
    }
    private IEnumerator DeadToGround()
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = EnemyBody.position + Vector3.down * 5;
        Destroy(GetComponentInParent<Rigidbody>());
        while (elapsedTime < sinkDuration)
        {
            EnemyBody.position = Vector3.Lerp(EnemyBody.position, targetPosition, elapsedTime / sinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(GetComponentInParent<moveAgent>().gameObject);
    }
}
