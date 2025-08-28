using UnityEngine;
using UnityEngine.EventSystems;

public class CreateTower : MonoBehaviour, IPointerClickHandler
{
    public Vector3 CreateOffset;
    public Transform createPoint;
    public GameObject tower;
    private void Start()
    {
        createPoint = transform;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("build");
        Vector3 createStartPoint = new Vector3(createPoint.position.x + CreateOffset.x, createPoint.position.y + CreateOffset.y, createPoint.position.z + CreateOffset.z);
        Instantiate(tower, createStartPoint, Quaternion.identity);
    }
}
