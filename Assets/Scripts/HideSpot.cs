using UnityEngine;

public class HideSpot : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hidePoint;
    [SerializeField] Transform exitPoint;

    PlayerController player;
    CharacterController cc;
    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out player))
        {
            return; // 플레이어 아니면 반환
        }

        cc = interactor.GetComponent<CharacterController>(); // Interact에서 플레이어 정보 받았을때 플레이어 정보 가져옴

        if (player.IsHidden) // 숨어있음
        {
            Exit(); // 탈출
        }
        else
        {
            Hide(); // 숨음
        }


    }
    void Hide()
    {
        cc.enabled = false; // 물리 끄기
        player.transform.position = hidePoint.position;
        player.SetHidden(true); // 숨음
    }
    void Exit()
    {
        player.transform.position = exitPoint.position;
        cc.enabled = true; // 물리 켜기
        player.SetHidden(false); // 탈출
    }
}
