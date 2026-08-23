using UnityEngine;

public class PaperInteraction : MonoBehaviour, IInteractable
{
    [Header("UI Settings")]
    [SerializeField] private GameObject paperUI;

    [Header("Player Settings")]
    [SerializeField] private PlayerController playerController;

    public void Interact(GameObject interactor)
    {
        if (paperUI.activeSelf) // 이미 종이를 보고 있으면 닫기
        {
            CloseUI();
            return;
        }

        // 종이 UI 열기
        paperUI.SetActive(true);

        // 마우스 커서 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 플레이어 움직임 정지
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }
    private void Update()
    {
        // 종이를 보는 중에 플레이어가 죽으면 UI 닫기
        if (paperUI.activeSelf && playerController != null && playerController.IsDead)
        {
            CloseUI();
        }
    }
    private void CloseUI()
    {
        paperUI.SetActive(false);

        // 마우스 커서 숨기기
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 플레이어 움직임 다시 활성화
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}