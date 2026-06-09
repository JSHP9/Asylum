using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ItemType currentItemType; // 현재 들고 있는 아이템 타입(문열때 사용하는 데이터)
    public GameObject heldItemObject; // 현재 들고 있는 아이템 객체 (손에서 버릴때 사용)
    // 아이템을 주우면 이 위치로 순간이동 시켜서 자식으로 붙일 거
    public Transform handTransform; // 손 위치
}
