using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    // 인스펙터 창에서 키인지 레드키인지 마우스로 고를 수 있음
    [SerializeField] private ItemType myItem = ItemType.Key;
    private Rigidbody rb;
    private BoxCollider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerInventory inv))
        {
            // 손에 이미 다른 아이템 들려있으면
            if (inv.currentItemType != ItemType.None)
            {
                // 기존 아이템 바닥에 버리면 됨
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
                transform.SetParent(inv.handTransform); // 내 부모(transform.SetParent)의 위치를 플레이어의 손(inv.handTransform)으로 지정
                transform.localPosition = Vector3.zero; // 로컬 위치 초기화
                transform.localRotation = Quaternion.identity; // 로컬 각도 초기화 (회전의 0)
            }
        }
    }
}
