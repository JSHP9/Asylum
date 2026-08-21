using UnityEngine;
using UnityEngine.AI;
using System.Collections; // 코루틴 용도

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
    private bool ignoreDoor = false; // 잠긴 문 무시
    [SerializeField] private float attackRange = 1.5f; // 공격 범위
    private bool isAttacking = false; // 공격 중인지

    [Header("Hearing")]
    private bool isInvestigating = false; // 소리 조사중
    private Vector3 noisePosition; // 소리 위치
    [SerializeField] private float investigateWaitTime = 0.5f; // 조사 대기 시간

    [Header("Sound")]
    [SerializeField] private AudioSource patrolAudioSource; // 평상시 배경음 AudioSource
    [SerializeField] private AudioSource chaseAudioSource; // 추격 배경음 AudioSource
    [SerializeField] private AudioSource sfxAudioSource; // 공격 효과음 AudioSource
    [SerializeField] private AudioClip patrolBGM; // 평상시 배경음
    [SerializeField] private AudioClip chaseBGM; // 추격 배경음
    [SerializeField] private AudioClip attackSound; // 공격 소리
    private bool isChasing = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        sight = GetComponent<EnemySight>();
        player = target.GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();

        // 게임 시작시 평상시 배경음 재생
        StartPatrolBGM();
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
    private IEnumerator Attack()
    { // 공격 코루틴
        if (isAttacking || player.IsDead) // 공격중 or 죽었으면 공격 하지마셈
            yield break;

        isAttacking = true;
        agent.ResetPath();
        animator.SetTrigger("Attack");

        // 공격 애니메이션에서 손들고 내려친 다음에 넘어지게
        yield return new WaitForSeconds(1.0f);

        if (sfxAudioSource != null && attackSound != null)
        {
            sfxAudioSource.PlayOneShot(attackSound);
        }

        player.Die(transform);

        yield return new WaitForSeconds(2.267f - 1.0f);

        isAttacking = false;
    }
    public void HearNoise(Vector3 position)
    {
        if (player == null)
            return;

        if (player.IsDead)
            return;

        if (isAttacking) // 이미 공격 중이면 소리 무시
            return;

        noisePosition = position;
        isInvestigating = true;

        isPatrolWaiting = false;
        waitTime = 0f;

        agent.SetDestination(noisePosition);
    }
    private void StartPatrolBGM()
    {
        if (patrolAudioSource == null || patrolBGM == null)
            return;

        if (!patrolAudioSource.isPlaying)
        {
            patrolAudioSource.clip = patrolBGM;
            patrolAudioSource.loop = true;
            patrolAudioSource.Play();
        }
    }
    private void StopPatrolBGM()
    {
        if (patrolAudioSource == null)
            return;

        if (patrolAudioSource.isPlaying)
        {
            patrolAudioSource.Stop();
        }
    }
    private void StartChaseBGM()
    {
        if (isChasing)
            return;

        isChasing = true;

        // 추격 시작하면 평상시 배경음 정지
        StopPatrolBGM();

        if (chaseAudioSource != null && chaseBGM != null)
        {
            chaseAudioSource.clip = chaseBGM;
            chaseAudioSource.loop = true;
            chaseAudioSource.Play();
        }
    }
    private void StopChaseBGM()
    {
        if (!isChasing)
            return;

        isChasing = false;
        if (chaseAudioSource != null)
        {
            chaseAudioSource.Stop();
        }
        // 추격 종료하면 평상시 배경음 다시 재생
        StartPatrolBGM();
    }
    private void Update()
    {
        if (player == null)
            return;
        if (player.IsDead)
        {
            StopChaseBGM();

            agent.ResetPath();
            animator.SetInteger("State", 0); // Idle

            return;
        }

        if (isAttacking)
            return;

        if (CheckDoor())
            return; // 문 여는중, 이동 x

        if (isInvestigating) // 조사 중
        {
            // 조사 중에는 추격 BGM을 끄고 평상시 배경음 유지
            StopChaseBGM();

            // 조사 중 플레이어 발견
            if (!player.IsHidden && sight.CanSeePlayer())
            {
                isInvestigating = false;
                isPatrolWaiting = false;
                waitTime = 0f;

                // 여기서 return하지 않음
                // 아래 Chase 로직으로 넘어감
            }
            else
            {
                animator.SetInteger("State", 2);
                agent.speed = 2.5f;
                agent.SetDestination(noisePosition);

                // 소리 위치 도착
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    animator.SetInteger("State", 0);

                    waitTime += Time.deltaTime;

                    if (waitTime >= investigateWaitTime)
                    {
                        waitTime = 0f;
                        isInvestigating = false;

                        ChooseNewDestination();
                    }
                }
                return;
            }
        }

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
            {
                StopChaseBGM(); // 추격 사운드 정지
                return;
            }
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            // 추격 시작
            StartChaseBGM();

            // 안죽었고 && 공격 범위 안에 들어오면 공격
            if (!player.IsDead && attackRange >= distanceToPlayer)
            {
                StartCoroutine(Attack());
                return;
            }
            animator.SetInteger("State", 2); // 추적 애니메이션
            agent.speed = 3.5f; // 추격 속도 3.5f
            agent.SetDestination(target.position);

            isPatrolWaiting = false;
            waitTime = 0f;
        }
        else
        {
            // 추격 종료
            StopChaseBGM(); // 추격 사운드 정지
            // 목적지 도착 체크
            if (!agent.pathPending &&
                agent.remainingDistance <= agent.stoppingDistance)
            {
                // 도착 후 잠깐 대기
                if (isPatrolWaiting)
                {
                    waitTime += Time.deltaTime;

                    animator.SetInteger("State", 0); // Idle

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