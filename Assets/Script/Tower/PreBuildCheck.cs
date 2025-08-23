using System.Collections.Generic;
using UnityEngine;

public class PreBuildCheck : MonoBehaviour
{
    public static CameraManager instance;
    int layerMask;
    public GameObject[] orinalBuild;
    public Dictionary<string, GameObject> builds = new Dictionary<string, GameObject>();
    public GameObject preBuildTowerPrefab;
    public GameObject preBuildTower;
    public LayerMask isFoundation;
    public bool waitBuilder;
    private Material preBuildTowerMaterial;
    public Material orinalMaterial;
    public bool OnBuild;
    // Start is called before the first frame update
    void Start()
    {
        instance = FindFirstObjectByType<CameraManager>();
        waitBuilder = false;
        // 定义要跳过的层
        int layerToSkip = LayerMask.NameToLayer("PreBuildLayer");
        // 排除这一层（取反）
        layerMask = ~(1 << layerToSkip);
        foreach (GameObject build in orinalBuild)
        {
            builds.Add(build.gameObject.name, build);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waitBuilder == true && instance.currentCamera != null && preBuildTower != null)
        {
            Ray ray = instance.currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (((1 << hit.transform.gameObject.layer) & isFoundation) != 0)
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
        }
        if (Input.GetKeyDown(KeyCode.B) && waitBuilder == false)
        {
            if (!OnBuild)
            {
                UI.instance.inGameUI.OnBuildUI();
                OnBuild = true;
            }
            else
            {
                UI.instance.inGameUI.OffBuildUI();
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
            UI.instance.inGameUI.OffBuildUI();
        }
        if (Input.GetMouseButtonDown(0) && waitBuilder == true)
        {
            waitBuilder = false;
            if (preBuildTower != null && preBuildTowerPrefab != null)
            {
                Ray ray = instance.currentCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    // 检查是否击中了我们感兴趣的正方体
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
                        Destroy(preBuildTower);
                    }
                    else
                    {
                        Destroy(preBuildTower);
                    }
                }
                preBuildTower = null;
                preBuildTowerMaterial = null;

            }
            UI.instance.inGameUI.OffBuildUI();
        }
    }

    public void PreMachineGunBuild()
    {
        if (builds.TryGetValue("TowerMachineGun", out GameObject build) && UI.instance.GetComponentInChildren<TowerAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponentInChildren<DefenseTower>().setCanAttack(false);
            preBuildTowerMaterial = preBuildTower.GetComponentInChildren<Renderer>().material;
            orinalMaterial = new Material(preBuildTowerMaterial);
            UI.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UI.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
    public void PreSolarPowerBuild()
    {
        if (builds.TryGetValue("SolarPower", out GameObject build) && UI.instance.GetComponentInChildren<PowerAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponent<SourceTower>().SetCanCollect(false);
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
            UI.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UI.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
    public void PreDrillBitIronBuild()
    {
        if (builds.TryGetValue("DrillBitIron", out GameObject build) && UI.instance.GetComponentInChildren<PowerAsset>().GetCanBuild())
        {
            preBuildTowerPrefab = build;
            waitBuilder = true;
            preBuildTower = Instantiate(preBuildTowerPrefab, transform.position, Quaternion.identity);
            preBuildTower.GetComponent<DrillTower>().SetCanCollect(false);
            Transform cTransform = preBuildTower.transform.Find("tower_hammer/tower_hammer");
            if (cTransform != null)
            {
                preBuildTowerMaterial = cTransform.GetComponent<Renderer>().material;
            }
            else
            {
                preBuildTowerMaterial = preBuildTower.GetComponentInChildren<Renderer>().material;

            }
            orinalMaterial = new Material(preBuildTowerMaterial);
            UI.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
        else
        {
            Debug.Log("不存在这个塔");
            UI.instance.inGameUI.OffBuildUI();
            OnBuild = false;
        }
    }
}
