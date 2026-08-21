using UnityEngine;
using System.Collections; // 코루틴 용도

public class Door : MonoBehaviour, IInteractable
{
    // [SerializeField] private ItemType requiredKey = ItemType.None; // 기본값은 기본 문.
    [SerializeField] private bool isLocked = true; // Inspector에서 문마다 잠금 여부 설정
    public bool IsLocked => isLocked; // 외부에서는 읽기만 가능, 수정은 Door 내부의 isLocked를 통해서만 가능(이거도 프로퍼티임)
    public bool IsOpen { get; private set; } = false; // public으로 외부 접근은 해야하는데 수정은 내부에서만 가능해야할때 프로퍼티 씀.
    public bool IsAnimating { get; private set; } = false; // 문열리는 중에 e키 눌림 방지, 안하면 문이 발광함
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    public void Interact(GameObject interactor)
    {
        if (IsLocked)
        {
            Debug.Log("문이 잠겨있음. 자물쇠를 먼저 풀어야함");
            return;
        }

        if (IsAnimating)
            return;

        IsOpen = !IsOpen;
        if (audioSource != null)
        {
            if (openSound != null)
                audioSource.PlayOneShot(openSound);

            EnemyAI enemyAI = FindFirstObjectByType<EnemyAI>();

            if (enemyAI != null)
                enemyAI.HearNoise(transform.position);
        }
        Quaternion targetRotation = IsOpen ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.Euler(0f, 0f, 0f); // 문 열기 : 문 닫기 목표 각도 설정

        StartCoroutine(SmoothDoor(targetRotation));
    }
    private IEnumerator SmoothDoor(Quaternion targetRotation)
    {
        IsAnimating = true;
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
        IsAnimating = false;
    }
    public void UnlockDoor()
    {
        isLocked = false; // 이제 일반 문이 됨.
    }
}
