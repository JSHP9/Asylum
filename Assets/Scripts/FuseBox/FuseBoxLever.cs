using UnityEngine;

public class FuseBoxLever : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private string triggerName = "PullLever";

    [Header("전기 파티클 설정")]
    [SerializeField] private GameObject electricSpark; // 여기에 상시 켜져 있는 파티클을 넣으세요!

    public void PlayAnimation()
    {
        // 1. 애니메이션 재생
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger(triggerName);
            Debug.Log("레버 애니메이션 재생!");
        }

        // 2. 파티클 끄기
        if (electricSpark != null)
        {
            electricSpark.SetActive(false); // 전기 파티클 비활성화
            Debug.Log("전기 효과 종료!");
        }
    }
}