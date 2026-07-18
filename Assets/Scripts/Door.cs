using UnityEngine;
using System.Collections; // 코루틴 용도

public class Door : MonoBehaviour, IInteractable
{
    // [SerializeField] private ItemType requiredKey = ItemType.None; // 기본값은 기본 문.
    public bool isLocked = true;
    private bool isOpen = false;
    private bool isAnimating = false; // 문열리는 중에 e키 눌림 방지, 안하면 문이 발광함 
    public void Interact(GameObject interactor)
    {
        if (isLocked)
        {
            Debug.Log("문이 잠겨있음. 자물쇠를 먼저 풀어야함");
            return;
        }

        if (isAnimating)
            return;

        //// 잠김 확인 로직
        //if (requiredKey != ItemType.None)
        //{
        //    if (interactor.TryGetComponent(out PlayerInventory inv)) // 플레이어 인벤토리에 뭐 들었는지 가져옴
        //    {
        //        if (inv.currentItemType == requiredKey) // 플레이어 인벤토리에 있는게 현재 요구하고 있는 열쇠랑 일치함
        //        {
        //            // 문 열림
        //            Debug.Log("문열림");
        //            requiredKey = ItemType.None; // 자물쇠를 부서버림 (이제 일반 문임)
        //        }
        //        else
        //        {
        //            Debug.Log("ㅈ까십쇼, 열쇠가 다릅니다" + requiredKey + " 이걸로 여셈");
        //            return;
        //        }

        //    }
        //}

        isOpen = !isOpen;
        Quaternion targetRotation = isOpen ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.Euler(0f, 0f, 0f); // 문 열기 : 문 닫기 목표 각도 설정

        StartCoroutine(SmoothDoor(targetRotation));
    }
    private IEnumerator SmoothDoor(Quaternion targetRotation)
    {
        isAnimating = true;
        Quaternion startRotation = transform.localRotation; // 시작 각도
        float time = 0f;
        float duration = 1.5f; // 문 열리는 시간

        while (time < duration)
        {
            time = time + Time.deltaTime;
            // Quaternion.Slerp(시작각도, 목표각도, 진행률(time/duration))을 적용. Slerp(구면 선형 보간: 구면 상의 두 점 사이를 일정한 속도로 보간하는 함수)
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, (time / duration));
            // 여기까지 실행하고 다음 프레임까지 대기
            yield return null;
        }
        // Slerp는 부동소수점 오차때문에 오차가 있을 수 있다함, 따라서 문 오차 잡기 위해 목표 각도로 확실하게 고정
        transform.localRotation = targetRotation;
        isAnimating = false;
    }
    public void UnlockDoor()
    {
        Debug.Log("UnlockDoor 호출됨");
        isLocked = false; // 이제 일반 문이 됨.
    }
}
