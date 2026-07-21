using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemySight sight;
    [SerializeField] private Transform target;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
    }
    private void Update()
    {
        if (target == null)
            return;
        if (sight.CanSeePlayer())
        {
            // 매 프레임마다 agent에게 target의 위치로 가라고 목표지점 설정 명령 내림
            agent.SetDestination(target.position);
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) // agent의 경로를 계산하지 않고 있고 && 목적지까지 남은 거리가 randomPosition 이하일 때, stoppingDistance는 지정 거리 안에만 들어오면 도착한걸로간주함.
            { // 랜덤 위치 배회

                NavMeshHit hit; // 랜덤 위치 저장 변수
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * Random.Range(5, 10); // 랜덤 거리 저장
                randomPosition.y = transform.position.y; // Random.insideUnitSphere가 x,y,z 전부 반환해줘서(반지름 1인 구 안의 랜덤한 점을 뽑는거라서 그럼) 높이는 현재 높이로 맞춰줌
                                                          // AI주변 랜덤 위치 생성NavMesh.SamplePosition(기준 좌표, 찾은 NavMesh 위치 저장 변수, 제한 반경, 어떤 NavMesh 영역 쓸지);
                if (NavMesh.SamplePosition(randomPosition, out hit, 10, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position); // 거기로 이동
                }
            }
        }
    }
}
