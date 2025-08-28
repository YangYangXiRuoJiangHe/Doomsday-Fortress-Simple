using UnityEngine;
using System.Collections.Generic;
public class CreateBasicFound : MonoBehaviour
{
    public int X = 0;
    public int Y = 0;
    public int cubeScale = 1;
    public Transform CreateTransform;
    public GameObject Cube;
    public List<GameObject> CubeList;
    public bool isEmpty;
    private void Start()
    {
    }
    private void Update()
    {
    }

    [ContextMenu("CreateBasicFound")]
    private void CreateBasicCube()
    {
        if(Cube == null)
        {
            Debug.Log("Cube Prefab not find");
            return;
        }
        transform.localScale =new Vector3( cubeScale, cubeScale, cubeScale);
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                if(i == 0 && j == 0)
                {
                    continue;
                }
                Vector3 CreatePosition = new Vector3(CreateTransform.position.x + i * cubeScale, CreateTransform.position.y, CreateTransform.position.z + j * cubeScale);
                GameObject CreateCube = Instantiate(Cube, CreatePosition,Quaternion.identity,transform);
                CubeList.Add(CreateCube);
            }
        }
    }
    [ContextMenu("ClearBasicFound")]
    private void ClearBasicFound()
    {
        if(CubeList.Count == 0)
        {
            Debug.Log("not find alraad create cube");
            return;
        }
        foreach(GameObject cube in CubeList)
        {
            DestroyImmediate(cube);
        }
        CubeList.Clear();
    }
}
