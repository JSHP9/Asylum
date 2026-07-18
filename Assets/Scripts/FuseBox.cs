using UnityEngine;
using UnityEngine.Events; // 퍼즐, 문, 버튼, 컷신 등 Inspector에서 연결하고 싶은 것에  유니티 이벤트 씀, 코드끼리만 통신하면 Action 사용.

public class FuseBox : MonoBehaviour, IInteractable
{
    [Header("Events")]
    public UnityEvent onFuseExtracted; // 인스펙터에서 전깃줄 제거 함수 연결
    private bool isFuseExtracted = false; // 중복 방지 실행용

    public void Interact(GameObject interactor)
    {
        if (isFuseExtracted)
        {
            Debug.Log("이미 퓨즈 뽑힘");
            return;
        }

        if (interactor.TryGetComponent(out PlayerInventory inv)) // true false값 반환 만 하는게 아닌, out 으로 컴포넌트 inv 반환
        {
            if (inv.currentItemType == ItemType.InsulatedGlove)
            {
                Debug.Log("퓨즈 뽑음");
                isFuseExtracted = true;

                // 전깃줄 제거 이벤트 invoke
                onFuseExtracted.Invoke();
            }
            else
            {
                Debug.Log("감전되서 사망!, 나중에 플레이어 죽는거 구현할때 추가할 예정.");
            }
        }
    }
}
