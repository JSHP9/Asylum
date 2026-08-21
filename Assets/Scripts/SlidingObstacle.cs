using System.Collections;
using UnityEngine;

public class SlidingObstacle : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 targetPosOffset = new Vector3(0.7f, 0f, 0f);
    private bool isOpen = false;
    public bool IsOpen => isOpen;
    [SerializeField] private bool isLocked = true; // Inspector에서 잠금 여부 설정
    public bool IsLocked => isLocked; // 외부에서는 읽기만 가능
    private bool isAnimating = false; // 애니메이션 중에 e키 눌림 방지
    public bool IsAnimating => isAnimating;
    private Vector3 startPositon;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip Sound;
    private void Awake()
    {
        startPositon = transform.localPosition;
    }
    public void Interact(GameObject interactor)
    {
        if (isLocked)
        {
            Debug.Log("비밀번호 먼저 해제해야함.");
            return;
        }

        if (isAnimating)
            return;

        isOpen = !isOpen;
        // 문 밀리는 소리
        if (audioSource != null && Sound != null)
        {
            audioSource.PlayOneShot(Sound);
        }
        Vector3 targetPosition = isOpen ? startPositon + targetPosOffset : startPositon; // 장애물 밀기 : 장애물 밀기 목표 위치 설정

        StartCoroutine(SlideObstacle(targetPosition));
    }
    private IEnumerator SlideObstacle(Vector3 targetPosition)
    {
        isAnimating = true;
        Vector3 startPosition = transform.localPosition; // 시작 위치
        float time = 0f;
        float duration = 1.0f; // 밀리는 시간

        while (time < duration)
        {
            time = time + Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, (time / duration));
            // 여기까지 실행하고 다음 프레임까지 대기
            yield return null;
        }
        // Slerp는 부동소수점 오차때문에 오차가 있을 수 있다함, 따라서 문 오차 잡기 위해 목표 각도로 확실하게 고정
        transform.localPosition = targetPosition;
        isAnimating = false;
    }
    public void UnlockObstacle()
    {
        Debug.Log("UnlockObstacle 호출됨");
        isLocked = false; // 이제 열림.
        Interact(null); // 자동으로 열림
    }
}
