using Unity.VisualScripting;
using UnityEngine;

public class CupInteraction : MonoBehaviour, IInteractable
{
    [Header("연결 설정")]
    [SerializeField] private Animator doorAnimator;       // 문이나 Fusebox에 있는 Animator
    private bool isOpen = false;
    private bool isAnimating = false;

    public bool IsOpen => isOpen;
    public bool IsAnimating => isAnimating;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;

    private void Awake()
    {
        // 만약 인스펙터에서 애니메이터를 안 넣었다면 부모(Fusebox)나 본체에서 자동으로 찾기
        if (doorAnimator == null)
        {
            doorAnimator = GetComponentInParent<Animator>();
        }
    }

    // 플레이어가 상호작용 광선을 맞추고 키를 눌렀을 때 기존 시스템에 의해 호출됨
    public void Interact(GameObject interactor)
    {
        if (isAnimating)
            return;

        // 상태 반전 및 애니메이션 트리거 실행
        isOpen = !isOpen;
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        if (interactor.TryGetComponent<PlayerController>(out _))
        {
            EnemyAI enemyAI = FindFirstObjectByType<EnemyAI>();

            if (enemyAI != null)
            {
                enemyAI.HearNoise(transform.position);
            }
        }
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("ToggleDoor");
            Debug.Log(isOpen ? "문이 열립니다." : "문이 닫힙니다.");
        }
        else
        {
            Debug.LogWarning("Door Animator가 연결되지 않았습니다!");
        }
    }
}