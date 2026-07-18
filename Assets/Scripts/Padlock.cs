using UnityEngine;
using UnityEngine.Events;
using System.Collections; // 코루틴 용도

public class Padlock : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemType requiredKey; // 사용할 열쇠
    private Rigidbody rb;
    // 인스펙터에서 문 여는 함수를 연결할 공간
    public UnityEvent onUnlock;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Interact(GameObject interactor)
    {
        Debug.Log("Padlock Interact");
        // Door.cs에 있던 인벤토리 열쇠 검사 로직
        // 잠김 확인 로직
        if (requiredKey != ItemType.None)
        {
            if (interactor.TryGetComponent(out PlayerInventory inv)) // 플레이어 인벤토리에 뭐 들었는지 가져옴
            {
                if (inv.currentItemType == requiredKey) // 플레이어 인벤토리에 있는게 현재 요구하고 있는 열쇠랑 일치함
                {
                    // 문 열림
                    Debug.Log("자물쇠 풀림");
                    onUnlock.Invoke(); // 열리라는 신호
                    transform.SetParent(null);
                    rb.isKinematic = false; // 자물쇠 해제시 바닥에 떨어짐
                    gameObject.layer = LayerMask.NameToLayer("Padlock"); // 문 열리면 레이어 바꾼뒤에 플레이어랑 충돌 막음.
                    StartCoroutine(DisablePadlock());
                }
                else
                {
                    Debug.Log("열쇠가 다름 " + requiredKey + " 이걸로 여셈");
                    return;
                }

            }
        }
    }
    private IEnumerator DisablePadlock()
    {
        yield return new WaitForSeconds(3f); // 3초뒤

        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // 바닥에 떨어지면 나는 소리(이거듣고 ai가 찾아옴)
        // 나중에 구현
    }
}