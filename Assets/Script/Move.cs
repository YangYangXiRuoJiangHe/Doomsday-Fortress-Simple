using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 move;
    public float moveSpeed;
    public Vector3 rotate;
    public float rotateSpeed;
    public bool canMove;
    public bool canRotate;
    // Update is called once per frame
    protected virtual void Update()
    {
        if (canMove)
        {
            TransformMove();

        }
        if (canRotate)
        {

            TransformRotate();
        }
    }
    public void TransformMove()
    {
        Vector3 transformPosition = transform.position;
        transform.position = new Vector3(transformPosition.x + move.x * moveSpeed * Time.deltaTime, transformPosition.y + move.y * moveSpeed * Time.deltaTime, transformPosition.z + move.z * moveSpeed * Time.deltaTime);
    }
    public virtual void TransformRotate()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}
