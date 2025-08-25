using UnityEngine;

public class ZombieWaveManager : MonoBehaviour
{
    public static ZombieWaveManager instance;
    public GameObject zombieSpawnPoint;
    public ZombieSpawnPoint[] zombieSpawnPoints;
    //全局几秒生成多少只
    public float generateSpeed = 1;
    //总共生成多少只
    public int zombieTotalNumber = 0;
    public float generateTime;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        zombieSpawnPoints = zombieSpawnPoint.GetComponentsInChildren<ZombieSpawnPoint>();
    }
    private void Start()
    {
        generateTime = zombieTotalNumber * generateSpeed + 10;
        InvokeRepeating(nameof(StartGenerateZombie), 10f, generateTime);

    }
    [ContextMenu("StartGenerateZombie")]
    public void StartGenerateZombie()
    {
        SetZombieGenerate(zombieTotalNumber, generateSpeed);
    }
    [ContextMenu("EndGenerateZombie")]
    public void EndGenerateZombie()
    {
        SetZombieGenerate(0, 0);
    }

    public void SetZombieGenerate(int number, float speed)
    {
        int zombieNumber = number;
        int i = 0;
        int zombieSpawnCount = zombieSpawnPoints.Length;
        float currentGenerateSpeed = speed * zombieSpawnCount;
        foreach (ZombieSpawnPoint zsp in zombieSpawnPoints)
        {
            int currentGenerateNumber = (int)(zombieNumber / (zombieSpawnCount - i));
            zsp.SetCreateNumber(currentGenerateNumber);
            zsp.SetCreateDelay(currentGenerateSpeed);
            zsp.CreateZombies();
            zombieNumber -= currentGenerateNumber;
            i += 1;
        }
    }


}
