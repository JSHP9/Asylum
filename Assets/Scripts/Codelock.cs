using TMPro; // 텍스트 제어용
using UnityEngine;
using UnityEngine.Events;

public class Codelock : MonoBehaviour, IInteractable
{
    [Header("UI Settings")]
    [SerializeField] private GameObject codeLockUI; //  캔버스 패널
    [SerializeField] private TextMeshProUGUI passwordDisplay; // 누른 번호 보여줄 텍스트

    [Header("Lock Settings")]
    [SerializeField] private string correctPassword = "1694"; // 코드 비밀번호
    private string currentInput = ""; // 현재까지 누른 번호

    public UnityEvent onUnlock; // 정답 맞추면 invoke할 문

    [Header("Player Settings")]
    [SerializeField] private PlayerController playerController; // 플레이어 움직임 제어하는 스크립트

    // 상호작용
    public void Interact(GameObject interactor)
    {
        if (codeLockUI.activeSelf) //  이미 활성화 되어있으면 화면 나감 E 한번 더 누르기로 나가기
        {
            CloseUI();
            return;
        }
        // UI를 화면에 띄워줌
        codeLockUI.SetActive(true);
        currentInput = ""; // 켤 때마다 입력창 초기화
        UpdateDisplay();

        // 마우스를 화면에 보이게
        Cursor.visible = true;
        // 마우스 커서 잠금 해제해서 자유롭게 움직이게 함
        Cursor.lockState = CursorLockMode.None;

        if (playerController != null) {
            playerController.enabled = false;
        }
    }

    // 숫자 버튼
    public void AddNumber(string number)
    {
        // 번호가 4자리 미만일 때 currentInput에 방금 누른 number를 이어 붙임
        if (currentInput.Length < 4)
        {
            currentInput += number;
        }
        // UpdateDisplay() 호출해서 화면 갱신
        UpdateDisplay();
    }

    // Enter 버튼
    public void CheckPassword()
    {
        if (currentInput == correctPassword) // 정답
        {
            onUnlock.Invoke();
            codeLockUI.SetActive(false);
            CloseUI();
        }
        else // 틀림
        {
            Debug.Log("틀림: 삐빅사운드 언젠가 넣을 예정");
            currentInput = ""; // 초기화
            UpdateDisplay(); // 화면 갱신
        }
    }

    // Clear 버튼
    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    // 화면 갱신
    private void UpdateDisplay()
    {
        passwordDisplay.text = currentInput;
    }

    private void CloseUI()
    {
        codeLockUI.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}