using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        if (target == null)
            return;
        // 매 프레임마다 agent에게 target의 위치로 가라고 목표지점 설정 명령 내림
        agent.SetDestination(target.position);
    }
}
