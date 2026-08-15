using UnityEngine;

public class FuseBoxDoor : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    void Awake()
    {
        // 내 오브젝트(또는 자식)에 붙은 Animator 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();
    }

    // 플레이어가 E키 등으로 상호작용했을 때 호출할 함수
    public void ToggleDoor()
    {
        // 현재 상태를 반대로 뒤집음 (false -> true / true -> false)
        isOpen = !isOpen;

        // 애니메이터의 isOpen (Bool) 파라미터를 변경하여 애니메이션 실행!
        animator.SetBool("isOpen", isOpen);
    }
}
