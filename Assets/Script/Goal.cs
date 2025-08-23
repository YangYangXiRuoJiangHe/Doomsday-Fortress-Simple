using UnityEngine;

public class Goal : MonoBehaviour
{
    int i;
    private void OnTriggerEnter(Collider other)
    {
        ZombieData zombieDate = other.GetComponent<ZombieData>();
        if(zombieDate != null)
        {
            zombieDate.TakeDamage(9999);
            SourceManager.instance.AddSourceData("health", -1);
        }
    }
}
