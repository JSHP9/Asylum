using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalVelocity = 0f; // y축 속도를 저장하는 변수.
    private InputSystem_Actions input;
    private CharacterController cc; // rigidbody 대신 CharacterController로 교체
    private Vector2 moveInput; // 앞뒤, 좌우 두가지 축만 있기때문에 Vector2임.

    // Camera Settings
    [SerializeField] private Transform cameraPivot; // Camera pivot 드래그 해서 넣기
    public float lookSensitivity = 15f; // 마우스 감도

    private Vector2 lookInput;
    private float xRotation = 0f;

    [Header("Crouching")]
    private bool isCrouching = false;
    private float crouchSpeed = 5f; // 앉는 속도
    [SerializeField] private float standHeight = 2f; // 서있을때 키
    [SerializeField] private float crouchHeight = 1.2f; // 앉았을때 키
    [SerializeField] private float standCenter = 1f; // 서있을때 센터
    [SerializeField] private float crouchCenter = 0.6f; // 앉았을때 센터
    [SerializeField] private float standCamera = 1.6f; // 서있을때 카메라
    [SerializeField] private float crouchCamera = 1.0f; // 앉았을때 카메라
    public bool IsHidden { get; private set; } // 숨기 프로퍼티(PlayerController내에서만 수정 가능), 초기값 안넣으면 자동으로 기본값으로 false가 들어감.
    public HideSpot CurrentHideSpot { get; set; }

    [Header("Death")]
    [SerializeField] private Transform respawnPoint; // 플레이어가 부활할 위치
    [SerializeField] private int maxLives = 3; // 최대 목숨
    private int currentLives; // 현재 목숨
    private bool isDead = false; // 사망 중인지
    void  Awake()
    {
        cc = GetComponent<CharacterController>();
        // 객체 생성 (메모리 할당)
        input = new InputSystem_Actions();
        // Player 맵의 move 행동이 실행되면(performed), OnMove 함수를 호출해라.
        input.Player.Move.performed += OnMove;
        // 손가락을 떼면(canceled), 마찬가지로 OnMove를 호출. (값을 0으로 리셋하기 위해)
        input.Player.Move.canceled += OnMove;
        
        // 마우스/패드 스틱 입력 이벤트 구독(화면 움직임)
        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>(); // ctx는 지역변수
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        // 앉기 입력 이벤트
        input.Player.Crouch.performed += OnCrouch;

        currentLives = maxLives; // 목숨 초기화
    }
    // 생명주기 관리 (이거 안쓰면 게임 끄고나서도 메모리 줄줄 샘)
    private void OnEnable(){ if (input != null) input.Enable();  } // 입력 받기 시작
    private void OnDisable() { if (input != null) input.Disable(); } // 입력 받기 중지

    void Start()
    {
        // 마우스 커서를 게임 화면 정중앙에 잠금
        Cursor.lockState = CursorLockMode.Locked;
        // 마우스 커서를 투명하게 숨김 (Locked 상태면 자동으로 숨겨지기도 하지만 명시적으로 써주는 게 좋다함)
        Cursor.visible = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // context 안에 유저가 누른 x, y 좌표값이 들어있음 그걸 꺼내서 변수에 저장함.
        // 대각선 속도 방지 정규화하는건 inputSystem_Actions안에 mode에 들어있었음.
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnCrouch(InputAction.CallbackContext context)
    {
        // 쭈그림/서있음 상태 변경
        isCrouching = !isCrouching;
    }
    void Update()
    {
        if (IsHidden) // 숨어있는동안 못움직임
            return;

        if (isDead) // 죽었으면 못움직임
            return;

        // 마우스 입력값에 감도와 프레임 보정 시간(Time.deltaTime)을 곱해서 변수에 담기
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // 위아래 보기 (머리 끄덕이기) 로직
        xRotation -= mouseY; // mouseY값 누적
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // -90 ~ 90까지로 범위 제한 (목 뒤로 꺾임 방지)
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // cameraPivot.localRotation에 반영

        // 좌우 보기 (몸통 회전) 로직
        transform.Rotate(0f, mouseX, 0f); // 마우스 입력은 매 프레임마다 들어와서 업데이트에 넣음

        // 좌/우 + 앞뒤
        Vector3 moveDirection = (moveInput.x * transform.right) + (moveInput.y * transform.forward);
        moveDirection *= moveSpeed;

        // 중력 구현
        if (cc.isGrounded)
        { //  땅
            verticalVelocity = -2.0f;
        }
        else
        { // 공중
            verticalVelocity += Physics.gravity.y * Time.deltaTime; // 중력가속도 * 프레임
        }
        // 수평이동에 중력 합침(y대입)
        moveDirection.y = verticalVelocity;

        // 캐릭터 컨트롤러 사용
        cc.Move(moveDirection * Time.deltaTime); // CharacterController는 물리 안쓰니까 Update()임.


        if (isCrouching)
        {
            // Mathf.lerp(시작각도, 목표각도, 진행률)을 적용. 선형 보간: 두 점 사이를 일정한 속도로 보간하는 함수
            cc.height = Mathf.Lerp(cc.height, crouchHeight, crouchSpeed * Time.deltaTime); // 키를 절반에서 0.2정도 더한값으로 천천히 앉음
            // 센터를 절반에서 0.2정도 더한값으로 천천히 앉힘(센터도 같이 안내려가면 얘가 쪼그라들어서 공중에 뜸)
            cc.center = new Vector3(cc.center.x, (Mathf.Lerp(cc.center.y, crouchCenter, crouchSpeed * Time.deltaTime)), cc.center.z);
            // 카메라도 내림
            cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x, Mathf.Lerp(cameraPivot.localPosition.y, crouchCamera, crouchSpeed * Time.deltaTime), cameraPivot.localPosition.z);
        }
        else
        {
            cc.height = Mathf.Lerp(cc.height, standHeight, crouchSpeed * Time.deltaTime); 
            cc.center = new Vector3(cc.center.x, (Mathf.Lerp(cc.center.y, standCenter, crouchSpeed * Time.deltaTime)), cc.center.z);
            cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x, Mathf.Lerp(cameraPivot.localPosition.y, standCamera, crouchSpeed * Time.deltaTime), cameraPivot.localPosition.z);
        }
        
    }

    public void SetHidden(bool hidden)
    {
        IsHidden = hidden; // 숨기 상태 변경
    }

    public void Die(Transform attacker)
    {
        if (isDead) return;
        StartCoroutine(DeathSequence(attacker));
    }
    private IEnumerator DeathSequence(Transform attacker)
    {
        isDead = true;
        moveInput = Vector2.zero;

        // ai를 바라보도록 카메라 연출
        Vector3 directionToAttacker = attacker.position - cameraPivot.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToAttacker);
        //Vector3 targetPosition = attacker.position + attacker.forward * 1.0f + Vector3.up * 1.2f;
        //Quaternion targetRotation = Quaternion.LookRotation(attacker.position + Vector3.up * 1.2f - targetPosition);

        float time = 0f;
        float duration = 0.5f; // 회전 시간
        Quaternion startRotation = cameraPivot.rotation; //  현재 카메라 회전

        while (time < duration) // 0.5c초동안
        {
            time += Time.deltaTime;
            cameraPivot.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration); // 구면 선형 보간

            yield return null; // 한 프레임 쉬기, 이래야 매 프레임마다 카메라 값이 조금씩 바뀜
        }
        cameraPivot.rotation = targetRotation; // Slerp는 부동소수점 오차가 있을 수 있어서 마지막에 정확하게 고정하는게 국룰인거같음

        yield return new WaitForSeconds(1f); // 사망 연출 잠깐 유지(0.5초동안 카메라 바라봄 + 1초동안 유지)

        currentLives--;

        if (currentLives <= 0)
        { // 남은 목숨 없음
            Debug.Log("Game Over");
            yield break;
        }

        // 리스폰
        cc.enabled = false; // 순간이동 해야해서 cc 잠깐 꺼둠

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        xRotation = 0f;
        cameraPivot.localRotation = Quaternion.identity;

        cc.enabled = true;
        isDead = false;
    }
}
