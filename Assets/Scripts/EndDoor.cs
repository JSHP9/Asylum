using UnityEngine;

public class EndDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isLocked = true;
    [SerializeField] private GameObject endingUI; // 엔딩 UI임(인스펙터에서 나중에 연결)

    public void Interact(GameObject interactor)
    {
        if (isLocked)
        {
            Debug.Log("문이 잠겨있음.");
            return;
        }

        Debug.Log("탈출 성공!");
        if (endingUI != null)
        {
            endingUI.SetActive(true); // 엔딩 UI 등장
        }
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }
}
