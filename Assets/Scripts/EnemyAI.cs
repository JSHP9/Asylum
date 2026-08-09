using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemySight sight;
    private PlayerController player; // 플레이어의 PlayerController 스크립트(상태 / 기능)

    private float waitTime = 0f; // 주변 둘러보는 시간
    private bool isPatrolWaiting = false; // 주변 둘러보는 상태

    [SerializeField] private Transform target; // 플레이어의 Transform(위치/회전 정보)

    private bool wasHidden = false; // 이전 프레임에 숨었는지

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        player = target.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player == null)
            return;

        // 현재 숨었는지
        bool hiddenNow = player.IsHidden;

        // 숨기 상태에 처음 들어왔을 때 한 번만 실행
        if (hiddenNow)
        {
            if (!wasHidden)
            {
                // 현재 추적 경로 제거
                if (agent.hasPath)
                    agent.ResetPath();

                // 순찰 상태 초기화
                isPatrolWaiting = false;
                waitTime = 0f;

                wasHidden = true;
            }
        }
        else
        {
            // 다시 안 숨은 상태가 되면 초기화
            wasHidden = false;
        }

        // 숨지 않았고 시야에 보이면 추적
        if (!hiddenNow && sight.CanSeePlayer())
        {
            agent.SetDestination(target.position);
            isPatrolWaiting = false;
            waitTime = 0f;
        }
        else
        {
            // 목적지 도착 체크
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // 도착 후 잠깐 대기
                if (isPatrolWaiting)
                {
                    waitTime += Time.deltaTime;

                    // 나중에 LookAround 애니메이션 넣을 예정
                    if (waitTime >= 2f)
                    {
                        waitTime = 0f;
                        isPatrolWaiting = false;
                    }
                    else
                    {
                        return;
                    }
                }

                // 랜덤 순찰 위치 생성
                NavMeshHit hit;

                Vector3 randomPosition =
                    transform.position +
                    Random.insideUnitSphere * Random.Range(10f, 20f);

                randomPosition.y = transform.position.y;

                // NavMesh 위의 유효한 위치 찾기
                if (NavMesh.SamplePosition(randomPosition, out hit, 20f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    isPatrolWaiting = true;
                }
            }
        }
    }
}