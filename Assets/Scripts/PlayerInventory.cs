using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ItemType currentItemType = ItemType.None; // 현재 들고 있는 아이템 타입(문열때 사용하는 데이터)
    public GameObject heldItemObject = null; // 현재 들고 있는 아이템 객체 (손에서 버릴때 사용)
    // 아이템을 주우면 이 위치로 순간이동 시켜서 자식으로 붙일 거
    public Transform handTransform; // 손 위치
    // 아이템 버리기
    public void DropCurrentItem()
    {
        // 손에 든 게 없으면 버릴 것도 없으니 걍 무시함
        if (currentItemType == ItemType.None || heldItemObject == null) return;

        heldItemObject.transform.SetParent(null); // 열쇠의 부모를 끊음
        heldItemObject.transform.position = transform.position + Vector3.up * 1.5f + transform.forward * 1.2f; // 위쪽으로 1.5f 올려서 가슴부근에서 아이템 떨어지게 설정(아이템 바닥 꺼짐 방지)
        heldItemObject.layer = LayerMask.NameToLayer("Interactable"); // 버려지는 물건의 레이어를 다시 상호작용 가능한 레이어로 원복
        // 물리엔진 원복
        Rigidbody rb = heldItemObject.GetComponent<Rigidbody>();
        Collider col = heldItemObject.GetComponent<Collider>();
        rb.isKinematic = false;
        col.enabled = true;
        // 데이터 리셋
        currentItemType = ItemType.None; // 데이터 맨손으로 변경
        heldItemObject = null; // 현재 아이템 객체 비움

    }
}