using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float armLength = 3f;
    [SerializeField] private LayerMask interactLayer;
    private InputSystem_Actions inputInteract; // 상호작용 inputSystem

    private void Awake()
    {
        inputInteract = new InputSystem_Actions();
        inputInteract.Player.Interact.performed += OnInteractAction; // 상호작용은 한번만 누르면 끝이니까 cancled안만들었음
        inputInteract.Player.Drop.performed += AttemptDrop; // 버리기
    }

    private void OnEnable() { if (inputInteract != null) inputInteract.Enable(); }
    private void OnDisable() { if (inputInteract != null) inputInteract.Disable(); }

    private void OnInteractAction(InputAction.CallbackContext context)
    { // 지금 당장 매개변수 안쓰긴하는데 perfomed가 CallbackContext를 받는 함수만 등록 가능해서 씀
      // 사실 람다 써도 된다만 나중에 더 커질수 있어서 걍 함수로 뺐음
        AttemptInteract();
    }
    private void AttemptDrop(InputAction.CallbackContext context)
    { // perfomed가 매개변수 있는 함수 요구해서 씀, 재사용 가능성 때문에 입출력 분리해둔거 위와 같은 말임.
        AttemptDrop();
    }
    private void AttemptInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward); // 레이저 포인터
        // RaycastHit hit; // 레이저가 맞은 결과 저장용(물리 충돌 결과 보고서). 그냥 여기서 안쓰고 Raycast안에 넣었음(요즘 방식이라함)
        // Raycast(광선, 충돌 정보 컨테이너, 사정거리, 검사할 레이어)에서 광선이 시작지점, 방향인데 ray안에 시작지점이랑 방향 둘다 들어있어서 저리 가능함
        if (Physics.Raycast(ray, out RaycastHit hit, armLength, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>(); // 부모 객체까지 찾음
            if (interactable != null)
            {
                interactable.Interact(this.gameObject); // 열쇠가 인벤토리 찾을 수 있게 나를 던져줌 (왜 인벤토리를 던지지않냐->나중에 숨기기능같은거 만드려면 플레이어를 던지는게 맞음)
            }
            return; // 뭔가 맞았으면 끝냄
        }
    }
    private void AttemptDrop()
    {
        if (this.gameObject.TryGetComponent(out PlayerInventory inv))
        {
            // 인벤토리에 들고 있는 거 버리셈
            inv.DropCurrentItem();
        }
    }
}
