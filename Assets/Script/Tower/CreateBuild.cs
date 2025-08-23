using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class CreateBuild : MonoBehaviour//,IPointerClickHandler
{
    public GameObject build;
    public Transform createPoint;
    public CreateBasicFound createBasicFound;
    public bool canBuildTower;
    public List<GameObject> builds;
    private void Start()
    {
        createPoint = transform;
        createBasicFound = GetComponent<CreateBasicFound>();
    }

   /* public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("build");
        Vector3 createStartPoint = createPoint.position;
        createStartPoint.y += 1.5f;
        Instantiate(build, createStartPoint, Quaternion.identity);
    }*/
    [ContextMenu("CreateRangeBuild")]
    public void CreateRangeBuild()
    {
        int createRange = createBasicFound.CubeList.Count;
        int emptyPosition = (int)Random.Range(1, 10);
        for (int i = 0; i < createRange; i++)
        {
            if (i == emptyPosition)
            {
                emptyPosition = (int)Random.Range(i + 5, i + 15);
                continue;
            }
            if ((i+2)%createBasicFound.X ==0)
            {
                continue;
            }
            if(i+1+createBasicFound.Y > createRange)
            {
                return;
            }
            if (i % 2 == 0)
            {
                continue;
            }
            Vector3 createStartPoint = createBasicFound.CubeList[i].transform.position;
            createStartPoint.y += 1.5f;
            builds.Add(Instantiate(build, createStartPoint, Quaternion.identity,transform));
        }
    }
    [ContextMenu("ClearBuilds")]
    public void ClearBuilds()
    {
        foreach(GameObject build in builds)
        {
            DestroyImmediate(build.gameObject);
        }
        builds.Clear();
    }
}
