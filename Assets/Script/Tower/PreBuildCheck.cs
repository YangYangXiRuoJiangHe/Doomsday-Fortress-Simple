using System.Collections.Generic;
using UnityEngine;

public class PreBuildCheck : MonoBehaviour
{
    public static PreBuildCheck instance;
    public static CameraManager cameraManager;
    public GameObject[] orinalBuild;
    public Dictionary<string, GameObject> builds = new Dictionary<string, GameObject>();
    public GameObject preBuildTowerPrefab;
    public GameObject preBuildTower;
    public LayerMask isFoundation;
    [SerializeField] private bool waitBuilder;
    private List<Material> preBuildTowerMaterial = new List<Material>();
    public List<Material> originalMaterial = new List<Material>();
    public bool OnBuild;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
        waitBuilder = false;
        foreach (GameObject build in orinalBuild)
        {
            builds.Add(build.gameObject.name, build);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waitBuilder == true && cameraManager.currentCamera != null && preBuildTower != null)
        {
            Ray ray = cameraManager.currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, isFoundation))
            {
                Vector3 preBuildPositionOffset = preBuildTower.GetComponentInChildren<Tower>().GetBuildOffset();
                Vector3 preBuildPosition = new Vector3(hit.transform.position.x + preBuildPositionOffset.x, hit.transform.position.y + preBuildPositionOffset.y, hit.transform.position.z + preBuildPositionOffset.z);
                preBuildTower.transform.position = preBuildPosition;
                if (!preBuildTower.GetComponentInChildren<Tower>().EnableBuilding())
                {
                    for(int i = 0; i < preBuildTowerMaterial.Count; i++)
                    {
                        preBuildTowerMaterial[i].SetColor("_EmissionColor", Color.red);
                        preBuildTowerMaterial[i].color = Color.red;
                    }

                }
                else
                {
                    for (int i = 0; i < preBuildTowerMaterial.Count; i++)
                    {
                        preBuildTowerMaterial[i].SetColor("_EmissionColor", originalMaterial[i].color);
                        preBuildTowerMaterial[i].color = originalMaterial[i].color;
                    }
                }

            }
        }
        if (Input.GetKeyDown(KeyCode.B) && waitBuilder == false && CameraManager.instance.currentCamera)
        {
            if (UIManager.instance.inGameUI.BuildUI.gameObject.activeSelf)
            {
                UIManager.instance.inGameUI.OffBuildUI();
                OnBuild = true;
            }
            else
            {
                UIManager.instance.inGameUI.OnBuildUI();
                OnBuild = false;
            }
        }
        if (Input.GetMouseButtonDown(1) && waitBuilder == true)
        {
            waitBuilder = false;
            if (preBuildTower != null)
            {
                Destroy(preBuildTower);
            }
            preBuildTowerMaterial.Clear();
            originalMaterial.Clear();
            UIManager.instance.inGameUI.OffBuildUI();
        }
        if (Input.GetMouseButtonDown(0) && waitBuilder == true)
        {
            waitBuilder = false;
            if (preBuildTower != null && preBuildTowerPrefab != null)
            {
                Ray ray = cameraManager.currentCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, isFoundation))
                {
                    Destroy(preBuildTower);
                    // 检查是否击中了能创建的正方体
                    if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Foundation")
                    {
                        GameObject Tower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
                        Vector3 preBuildPosition = new Vector3(hit.transform.position.x + Tower.GetComponentInChildren<Tower>().GetBuildOffset().x, hit.transform.position.y + Tower.GetComponentInChildren<Tower>().GetBuildOffset().y, hit.transform.position.z + Tower.GetComponentInChildren<Tower>().GetBuildOffset().z);
                        Tower.transform.position = preBuildPosition;
                        if (!Tower.GetComponentInChildren<Tower>().EnableBuilding())
                        {
                            Destroy(Tower);
                        }
                        else
                        {
                            //转到当前位置的方块上
                            Tower.transform.SetParent(hit.transform);
                            //将塔范围的方块设置为非空
                            Tower.GetComponentInChildren<Tower>().SetFountionNotEmpty();
                            //设置资源管理器可以减去塔的创建资源（减去创建资源在tower的start中执行，而这行在实例化之前执行，晚于构造函数，早于awake）
                            Tower.GetComponentInChildren<Tower>().SetIsReduce(true);
                            //Tower.GetComponentInChildren<Tower>().ReduceResources();
                        }

                    }
                }
                preBuildTower = null;
                preBuildTowerMaterial.Clear();
                originalMaterial.Clear();

            }
            UIManager.instance.inGameUI.OffBuildUI();
        }
    }
    public void PreBuildingDefense(GameObject build)
    {
        if(build == null)
        {
            Debug.Log("请检查是否有这个塔");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
            return;
        }
        if (!build.GetComponentInChildren<Tower>().CanBuildTower())
        {
            Debug.Log("资源不足");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
            return;
        }
        preBuildTowerPrefab = build;
        waitBuilder = true;
        preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
        preBuildTower.GetComponentInChildren<DefenseTower>().setCanAttack(false);
        preBuildTower.GetComponentInChildren<DefenseTower>().SetIsReduce(false);
        preBuildTower.GetComponentInChildren<BoxCollider>().enabled = false;
        foreach(Renderer renderer in preBuildTower.GetComponentsInChildren<Renderer>())
        {
            preBuildTowerMaterial.Add(renderer.material);
            originalMaterial.Add(new Material(renderer.material));
        }
        UIManager.instance.inGameUI.OffBuildUI();
        OnBuild = false;
    }
    public void PreBuildingSource(GameObject build)
    {
        if (build == null)
        {
            Debug.Log("请检查是否有这个塔");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
            return;
        }
        if (!build.GetComponentInChildren<Tower>().CanBuildTower())
        {
            Debug.Log("资源不足");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
            return;
        }
        preBuildTowerPrefab = build;
        waitBuilder = true;
        preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
        preBuildTower.GetComponentInChildren<SourceTower>().SetCanCollect(false);
        preBuildTower.GetComponentInChildren<SourceTower>().SetIsReduce(false);
        preBuildTower.GetComponentInChildren<BoxCollider>().enabled = false;
        foreach (Renderer renderer in preBuildTower.GetComponentsInChildren<Renderer>())
        {
            preBuildTowerMaterial.Add(renderer.material);
            originalMaterial.Add(new Material(renderer.material));
        }
        UIManager.instance.inGameUI.OffBuildUI();
        OnBuild = false;
    }
    public bool GetWaitBuilder() => waitBuilder;
}
