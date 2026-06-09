using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private InputSystem_Actions input;
    private Rigidbody rb;
    private Vector2 moveInput; // 앞뒤, 좌우 두가지 축만 있기때문에 Vector2임.

    // Camera Settings
    [SerializeField] private Transform cameraPivot; // Camera pivot 드래그 해서 넣기
    public float lookSensitivity = 15f; // 마우스 감도

    private Vector2 lookInput;
    private float xRotation = 0f;
    void  Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 객체 생성 (메모리 할당)
        input = new InputSystem_Actions();
        // Player 맵의 move 행동이 실행되면(performed), OnMove 함수를 호출해라.
        input.Player.Move.performed += OnMove;
        // 손가락을 떼면(canceled), 마찬가지로 OnMove를 호출. (값을 0으로 리셋하기 위해)
        input.Player.Move.canceled += OnMove;
        // 마우스/패드 스틱 입력 이벤트 구독
        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>(); // ctx는 지역변수
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;
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
    private void FixedUpdate()
    {
        // 좌/우 + 앞뒤
        Vector3 moveDirection = (moveInput.x * transform.right) + (moveInput.y * transform.forward);
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
    void Update()
    {

        // 마우스 입력값에 감도와 프레임 보정 시간(Time.deltaTime)을 곱해서 변수에 담기
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // 위아래 보기 (머리 끄덕이기) 로직
        xRotation -= mouseY; // mouseY값 누적
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // -90 ~ 90까지로 범위 제한 (목 뒤로 꺾임 방지)
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // cameraPivot.localRotation에 반영

        // 좌우 보기 (몸통 회전) 로직
        transform.Rotate(0f, mouseX, 0f); // 마우스 입력은 매 프레임마다 들어와서 업데이트에 넣음
    }
}
