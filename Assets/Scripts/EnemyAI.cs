using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemySight sight;
    private float waitTime = 0f; // 주변 둘러보는 시간
    private bool isPatrolWaiting = false; // 주변 둘러보는 상태
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
            isPatrolWaiting = false;
            waitTime = 0f;
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) // agent의 경로를 계산하지 않고 있고 && 목적지까지 남은 거리가 randomPosition 이하일 때, stoppingDistance는 지정 거리 안에만 들어오면 도착한걸로간주함.
            {
                if (isPatrolWaiting) // 목적지 도착
                {
                    waitTime += Time.deltaTime;
                    // 나중에 Animator.SetTrigger("LookAround") 애니메이션 넣어서 좌우 둘러보는거 구현 예정
                    // + 뭔가 지금 if 지옥 되는거같아서 애니메이션 만들때 state노선 타거나 함수로 바꿔버릴 예정임.
                    if (waitTime >= 2f) // 2초이전까지 주변 둘러봄
                    {
                        waitTime = 0f;
                        isPatrolWaiting = false;
                    }
                    else
                        return;
                }

                // 랜덤 위치 
                NavMeshHit hit; // 랜덤 위치 저장 변수
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * Random.Range(10, 20); // 랜덤 거리 저장
                randomPosition.y = transform.position.y; // Random.insideUnitSphere가 x,y,z 전부 반환해줘서(반지름 1인 구 안의 랜덤한 점을 뽑는거라서 그럼) 높이는 현재 높이로 맞춰줌

                // AI주변 랜덤 위치 생성NavMesh.SamplePosition(기준 좌표, 찾은 NavMesh 위치 저장 변수, 제한 반경, 어떤 NavMesh 영역 쓸지);
                if (NavMesh.SamplePosition(randomPosition, out hit, 20, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position); // 거기로 이동
                    isPatrolWaiting = true;
                }
            }

        }
    }
}
