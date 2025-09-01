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
    private Material preBuildTowerMaterial;
    public Material orinalMaterial;
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
                    preBuildTowerMaterial.SetColor("_EmissionColor", Color.red);
                    preBuildTowerMaterial.color = Color.red;

                }
                else
                {
                    preBuildTowerMaterial.SetColor("_EmissionColor", orinalMaterial.color);
                    preBuildTowerMaterial.color = orinalMaterial.color;
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
            preBuildTowerMaterial = null;
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
                            Tower.GetComponentInChildren<Tower>().SetFountionNotEmpty();
                            Tower.GetComponentInChildren<Tower>().ReduceResources();
                            Tower.transform.SetParent(hit.transform);
                        }

                    }
                }
                preBuildTower = null;
                preBuildTowerMaterial = null;

            }
            UIManager.instance.inGameUI.OffBuildUI();
        }
    }

    public void PreMachineGunBuild()
    {
        if (builds.TryGetValue("TowerMachineGun", out GameObject build) && UIManager.instance.GetComponentInChildren<TowerMachineAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponentInChildren<DefenseTower>().setCanAttack(false);
            preBuildTower.GetComponentInChildren<BoxCollider>().enabled = false;
            preBuildTowerMaterial = preBuildTower.GetComponentInChildren<Renderer>().material;
            orinalMaterial = new Material(preBuildTowerMaterial);
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
    public void PreCannonBuild()
    {
        if (builds.TryGetValue("Tower_cannon", out GameObject build) && UIManager.instance.GetComponentInChildren<TowerMachineAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponentInChildren<DefenseTower>().setCanAttack(false);
            preBuildTower.GetComponentInChildren<BoxCollider>().enabled = false;
            Transform cTransform = preBuildTower.transform.Find("tower_cannon_rack/tower_cannon_cannon");
            if (cTransform != null)
            {
                preBuildTowerMaterial = cTransform.GetComponent<Renderer>().material;
            }
            else
            {
                Debug.Log(false);

                preBuildTowerMaterial = preBuildTower.GetComponentInChildren<Renderer>().material;

            }
            orinalMaterial = new Material(preBuildTowerMaterial);
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
    public void PreSolarPowerBuild()
    {
        if (builds.TryGetValue("SolarPower", out GameObject build) && UIManager.instance.GetComponentInChildren<SolarPowerAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponent<SourceTower>().SetCanCollect(false);
            preBuildTower.GetComponentInChildren<BoxCollider>().enabled = false;
            Transform cTransform = preBuildTower.transform.Find("msfmc_RadarDish/CommSattAttnMain");
            if (cTransform != null)
            {
                preBuildTowerMaterial = cTransform.GetComponent<Renderer>().material;
            }
            else
            {
                preBuildTowerMaterial = preBuildTower.GetComponentInChildren<Renderer>().material;

            }
            orinalMaterial = new Material(preBuildTowerMaterial);
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
    public void PreDrillBitIronBuild()
    {
        if (builds.TryGetValue("DrillBitIron", out GameObject build) && UIManager.instance.GetComponentInChildren<SolarPowerAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponent<DrillTower>().SetCanCollect(false);
            preBuildTower.GetComponentInChildren<BoxCollider>().enabled = false;
            Transform cTransform = preBuildTower.transform.Find("tower_hammer");
            if (cTransform != null)
            {
                preBuildTowerMaterial = cTransform.GetComponent<Renderer>().material;
            }
            else
            {
                preBuildTowerMaterial = preBuildTower.GetComponentInChildren<Renderer>().material;

            }
            orinalMaterial = new Material(preBuildTowerMaterial);
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UIManager.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
    public bool GetWaitBuilder() => waitBuilder;
}
