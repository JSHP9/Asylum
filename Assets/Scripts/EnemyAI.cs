using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemySight sight;
    private PlayerController player; // 플레이어의 PlayerController 스크립트(상태 / 기능)
    private Animator animator;
    private float waitTime = 0f; // 주변 둘러보는 시간
    private bool isPatrolWaiting = false; // 주변 둘러보는 상태
    [SerializeField] private Transform target; // 플레이어의 Transform(위치/회전 정보)
    [SerializeField] private float doorLength = 1.5f;
    private bool wasHidden = false; // 이전 프레임에 숨었는지
    private bool ignoreDoor = false;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        player = target.GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
    }
    private bool CheckDoor()
    {
        if (ignoreDoor) // 잠긴문/장애물 무시하는 중이면 감지하지 않음
            return false;

        Ray ray = new Ray(transform.position, transform.forward); // 레이저 포인터

        if (Physics.Raycast(ray, out RaycastHit hit, doorLength))
        {
            Door door = hit.collider.GetComponentInParent<Door>();

            if (door != null)
            {
                if (door.IsAnimating)
                    return true; // 문 열리는 중이면 움직이지 않음

                // 잠겨있지 않고 && 닫혀있음
                if (!door.IsLocked && !door.IsOpen)
                {
                    door.Interact(gameObject); // 문 열기
                    return true;
                }

                // 잠긴 문
                if (door.IsLocked)
                {
                    ChooseNewDestination(); // 새로운 목적지 이동
                    ignoreDoor = true; // 잠긴 문 무시
                    return true;
                }
            }

            SlidingObstacle slidingObstacle =
                hit.collider.GetComponentInParent<SlidingObstacle>();

            if (slidingObstacle != null)
            {
                if (slidingObstacle.IsAnimating)
                    return true; // 장애물이 움직이는 중이면 움직이지 않음

                // 잠겨있지 않고 && 닫혀있음
                if (!slidingObstacle.IsLocked && !slidingObstacle.IsOpen)
                {
                    slidingObstacle.Interact(gameObject); // 장애물 열기
                    return true;
                }

                // 잠긴 장애물
                if (slidingObstacle.IsLocked)
                {
                    ChooseNewDestination(); // 새로운 목적지로 이동
                    ignoreDoor = true; // 잠긴 장애물 무시
                    return true;
                }
            }
        }

        return false;
    }
    private void ChooseNewDestination()
    {
        // 랜덤 순찰 위치 생성
        NavMeshHit hit;

        Vector3 randomPosition =
            transform.position +
            Random.insideUnitSphere * Random.Range(15f, 30f);

        randomPosition.y = transform.position.y;

        // NavMesh 위의 유효한 위치 찾기
        if (NavMesh.SamplePosition(randomPosition, out hit, 20f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            isPatrolWaiting = true;
        }
    }
    private void Update()
    {
        if (player == null)
            return;

        if (CheckDoor())
            return; // 문 여는중, 이동 x


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
            wasHidden = false; // 다시 안 숨은 상태가 되면 초기화
        }
        
        animator.SetInteger("State", 1); // 순찰
        agent.speed = 2.0f; // 순찰 속도 2.0f

        // 안 숨었고 시야에 보이면 추적
        if (!hiddenNow && sight.CanSeePlayer())
        {
            if (ignoreDoor) // 눈에 보여도 문이 막혀있으면 추적하면안됨
                return;

            animator.SetInteger("State", 2); // 추적 애니메이션
            agent.speed = 3.5f; // 추격 속도 3.5f
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

                    animator.SetInteger("State", 0); // Idle 애니메이션
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
                ignoreDoor = false; // 목적지 도착했으면 잠긴문 인식 못하게 하는거 풀어줌
                ChooseNewDestination(); // 새로 목적지 뽑기
            }
        }
    }
}