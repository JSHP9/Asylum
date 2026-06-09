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
    {
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
        // 버리기
        // 여기서 인벤토리(PlayerInventory)를 검사해서, 손에 쥐고 있는 열쇠의 부모를 끊고 바닥에 툭 떨어뜨리는 코드를 짜야 함
        if (this.gameObject.TryGetComponent(out PlayerInventory inv))
        {
            if (inv.currentItemType == ItemType.None) { return; } // 맨손일때 버리기 누르는거 방지
            if (inv.currentItemType != ItemType.None)
            {
                inv.heldItemObject.transform.SetParent(null); // 열쇠의 부모를 끊음
                // 카메라 위치에서 앞으로 1.5미터 떨어진 곳으로 열쇠를 순간이동 시킴 (플레이어와의 충돌로 인해 플레이어가 밀려나는거 방지)
                inv.heldItemObject.transform.position = cameraTransform.position + cameraTransform.forward * 1.5f;
                // 물리엔진 원복
                Rigidbody rb = inv.heldItemObject.GetComponent<Rigidbody>();
                Collider col = inv.heldItemObject.GetComponent<Collider>();
                rb.isKinematic = false;
                col.enabled = true;

                inv.currentItemType = ItemType.None; // 데이터 맨손으로 변경
                inv.heldItemObject = null; // 현재 아이템 객체 비움
            }
        }

    }
}
