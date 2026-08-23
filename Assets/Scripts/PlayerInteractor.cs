using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float armLength = 3f;
    [SerializeField] private LayerMask interactLayer;

    private InputSystem_Actions inputInteract; // 상호작용 inputSystem

    private PlayerController player; // 숨기 기능 구현때문에 추가

    [Header("Item Name UI")]
    private GameObject currentItemNameUI; // 현재 표시중인 아이템 이름 UI

    [Header("Interaction UI")] // 아이템 상호작용하면 뜨게 표시
    [SerializeField] private GameObject requireRedUI;
    [SerializeField] private GameObject requireBlueUI;

    private void Awake()
    {
        inputInteract = new InputSystem_Actions();

        inputInteract.Player.Interact.performed += OnInteractAction;
        // 상호작용은 한번만 누르면 끝나니까 canceled 안만들었음

        inputInteract.Player.Drop.performed += AttemptDrop;
        // 버리기

        player = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        if (inputInteract != null)
            inputInteract.Enable();
    }

    private void OnDisable()
    {
        if (inputInteract != null)
            inputInteract.Disable();
    }

    private void Update()
    {
        UpdateItemNameUI();
        UpdateInteractionUI();
    }

    private void UpdateItemNameUI()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        // 레이저 포인터

        // 숨은 상태에서는 아이템 이름 표시하지 않음
        if (player.IsHidden)
        {
            HideItemNameUI();
            return;
        }

        if (Physics.Raycast(ray, out RaycastHit hit, armLength, interactLayer))
        {
            ItemName itemName = hit.collider.GetComponentInParent<ItemName>();

            if (itemName != null)
            {
                // 이미 같은 UI를 표시하고 있으면 아무것도 하지 않음
                if (currentItemNameUI == itemName.ItemNameUI)
                    return;

                // 기존 UI 끄기
                HideItemNameUI();

                // 새로운 아이템 이름 UI 켜기
                currentItemNameUI = itemName.ItemNameUI;

                if (currentItemNameUI != null)
                {
                    currentItemNameUI.SetActive(true);
                }

                return;
            }
        }

        // 아이템을 바라보고 있지 않으면 UI 끄기
        HideItemNameUI();
    }

    private void HideItemNameUI()
    {
        if (currentItemNameUI != null)
        {
            currentItemNameUI.SetActive(false);
            currentItemNameUI = null;
        }
    }

    private void HideInteractionUI()
    {
        requireRedUI.SetActive(false);
        requireBlueUI.SetActive(false);
    }

    private void UpdateInteractionUI()
    {
        if (player.IsHidden)
        {
            HideInteractionUI();
            return;
        }

        // 일단 전부 끄기
        requireRedUI.SetActive(false);
        requireBlueUI.SetActive(false);

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, armLength, interactLayer))
        {
            Padlock padlock = hit.collider.GetComponentInParent<Padlock>();

            if (padlock != null)
            {
                if (player.TryGetComponent(out PlayerInventory inv))
                {
                    // 현재 들고 있는 키가 자물쇠가 요구하는 키와 같음
                    if (inv.currentItemType == padlock.RequiredKey)
                    {
                        // 필요한 키를 가지고 있으면 별도의 UI를 띄우지 않음
                    }
                    else
                    {
                        // 필요한 키가 Red Key인 경우
                        if (padlock.RequiredKey == ItemType.RedKey)
                        {
                            requireRedUI.SetActive(true);
                        }
                        // 필요한 키가 Blue Key인 경우
                        else if (padlock.RequiredKey == ItemType.BlueKey)
                        {
                            requireBlueUI.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    private void OnInteractAction(InputAction.CallbackContext context)
    {
        // 지금 당장 매개변수 안쓰긴하는데 performed가
        // CallbackContext를 받는 함수만 등록 가능해서 씀
        // 사실 람다 써도 된다만 나중에 더 커질 수 있어서 걍 함수로 뺐음

        AttemptInteract();
    }

    private void AttemptDrop(InputAction.CallbackContext context)
    {
        // performed가 매개변수 있는 함수 요구해서 씀
        // 재사용 가능성 때문에 입출력 분리해둔거 위와 같은 말임

        AttemptDrop();
    }

    private void AttemptInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        // 레이저 포인터

        if (player.IsHidden && player.CurrentHideSpot != null)
        {
            // 숨은상태 && 어디 HideSpot에 숨었는지 기록이 되어있음
            player.CurrentHideSpot.Interact(gameObject);
            // 현재 기록된 숨은 위치랑 상호작용

            return;
        }

        // RaycastHit hit;
        // 레이저가 맞은 결과 저장용(물리 충돌 결과 보고서)
        // 그냥 여기서 안쓰고 Raycast안에 넣었음

        // Raycast(광선, 충돌 정보 컨테이너, 사정거리, 검사할 레이어)
        // 광선이 시작지점, 방향인데 ray안에 시작지점이랑 방향 둘다 들어있어서 저리 가능함

        if (Physics.Raycast(ray, out RaycastHit hit, armLength, interactLayer))
        {
            IInteractable interactable =
                hit.collider.GetComponentInParent<IInteractable>();

            // 부모 객체까지 찾음

            if (interactable != null)
            {
                interactable.Interact(this.gameObject);
                // 열쇠가 인벤토리 찾을 수 있게 나를 던져줌
                // 왜 인벤토리를 던지지 않냐?
                // 나중에 숨기기능같은거 만드려면 플레이어를 던지는게 맞음
            }

            return;
            // 뭔가 맞았으면 끝냄
        }
    }

    private void AttemptDrop()
    {
        if (player.IsHidden)
            return;
        // 숨은 동안 드랍 금지

        if (this.gameObject.TryGetComponent(out PlayerInventory inv))
        {
            // 인벤토리에 들고 있는 거 버리셈
            inv.DropCurrentItem();
        }
    }
}