using System.Collections;
using UnityEngine;

public class ZombieSpawnPoint : MonoBehaviour
{
    public int createNumber = 0;
    public float createDelay = 0;
    public GameObject zombie;

    private void Start()
    {
    }
    public void CreateZombies()
    {
        StartCoroutine(nameof(CreateZombie));
    }
    IEnumerator CreateZombie()
    {

        for (int i = 0; i < createNumber; i++)
        {
            Vector3 gnenratePoint = new Vector3(transform.position.x + Random.Range(-2f,2f), transform.position.y, transform.position.z);
            Instantiate(zombie, gnenratePoint, transform.rotation);
            yield return new WaitForSeconds(createDelay);
        }
    }
    public void SetCreateNumber(int number) => createNumber = number;
    public int GetCreateNumber() => createNumber;
    public void SetCreateDelay(float speed) => createDelay = speed;
    public float GetCreateSpeed() => createDelay;
}
