using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    // 인스펙터 창에서 키인지 레드키인지 마우스로 고를 수 있음
    [SerializeField] private ItemType myItem = ItemType.Key;

    [Header("손에 들었을 때 설정")]
    [SerializeField] private Vector3 heldScale = new Vector3(2f, 2f, 2f);        // 손에 들었을 때 크기
    [SerializeField] private Vector3 heldRotation = new Vector3(0f, 90f, 0f);    // 손에 들었을 때 방향

    private Rigidbody rb;
    private BoxCollider col;

    private Vector3 originalScale;    // 원래 크기 기억용
    private Quaternion originalRotation; // 원래 각도 기억용

    // 기존 크기 & 각도 정보 저장
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();

        originalScale = transform.localScale;       // 시작할 때 원래 크기 저장
        originalRotation = transform.localRotation; // 시작할 때 원래 각도 저장
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerInventory inv))
        {
            // 손에 이미 다른 아이템 들려있으면
            if (inv.currentItemType != ItemType.None)
            {
                // 기존 아이템 바닥에 버리면 됨(!! 여기에 스케일, 각도 원복코드 넣으면 해결되지 않을까?)
                inv.DropCurrentItem();

            }
            // 손이 비어있을때만 줍기 가능
            if (inv.currentItemType == ItemType.None)
            {
                // 인벤토리 데이터 갱신
                this.gameObject.layer = 0; // Default 레이어로 변경해서 레이저 팀킬 방지
                inv.currentItemType = myItem;
                inv.heldItemObject = this.gameObject; // 자기자신 등록

                // 물리 끄기
                rb.isKinematic = true; // 물리 엔진 무시
                col.enabled = false; // 플레이어랑 충돌 방지

                // 부모 자식 맺기
                transform.SetParent(inv.handTransform);
                transform.localPosition = Vector3.zero;

                // 인스펙터에 설정한 손에 든 방향과 크기 적용
                transform.localRotation = Quaternion.Euler(heldRotation);
                transform.localScale = heldScale;
            }
        }
    }

    // PlayerInventory에서 호출할 원복 함수
    public void ResetTransform()
    {
        transform.localScale = originalScale;       // 원래 크기로 복구
        transform.localRotation = originalRotation; // 원래 각도로 복구
    }
}
