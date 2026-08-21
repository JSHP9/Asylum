using UnityEngine;
using UnityEngine.Events;

public class FuseBox : MonoBehaviour, IInteractable
{
    [Header("Events")]
    public UnityEvent onFuseExtracted;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fuseExtractSound;

    private bool isFuseExtracted = false;

    public void Interact(GameObject interactor)
    {
        if (isFuseExtracted)
        {
            Debug.Log("이미 퓨즈 뽑힘");
            return;
        }

        if (interactor.TryGetComponent(out PlayerInventory inv))
        {
            if (inv.currentItemType == ItemType.InsulatedGlove)
            {
                Debug.Log("퓨즈 뽑음");
                isFuseExtracted = true;

                // 퓨즈 뽑는 소리
                if (audioSource != null && fuseExtractSound != null)
                {
                    audioSource.PlayOneShot(fuseExtractSound);
                }

                // 플레이어가 퓨즈를 뽑았으므로 AI에게 소리 전달
                EnemyAI enemyAI = FindFirstObjectByType<EnemyAI>();

                if (enemyAI != null)
                {
                    enemyAI.HearNoise(transform.position);
                }

                // 전깃줄 제거 + 레버 애니메이션
                onFuseExtracted.Invoke();
            }
            else
            {
                Debug.Log("감전되서 사망!, 나중에 플레이어 죽는거 구현할때 추가할 예정.");
            }
        }
    }
}