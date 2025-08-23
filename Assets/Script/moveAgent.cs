using UnityEngine;
using UnityEngine.AI;

public class moveAgent : MonoBehaviour
{
    public Goal goal;
    NavMeshAgent agent;
    public float agentSpeed;
    public float animSpeed = 4f;
    public Animator anim;
    public Rigidbody rb;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        goal = FindAnyObjectByType<Goal>();
        agent.destination = goal.transform.position;
        rb = GetComponentInParent<Rigidbody>();
       /* agent.updatePosition = false; //animator�п���apply root motion
        agent.updateRotation = true; // �����Ҫ��ת�Ļ�*/

    }
    private void Update()
    {
        agent.speed = agentSpeed;
        if (agent == null || anim == null) return;
        agent.SetDestination(goal.transform.position);
        //OnAnimatorMove();
    }
    public void OnAnimatorMove()
    {
        Vector3 deltaPosition = anim.deltaPosition; // ����������λ��
        rb.MovePosition(rb.position + deltaPosition);
    }
    public void SetAgentSpeed(float speed)
    {
        agent.speed = speed;
    }
}
