using UnityEngine;

public class HideSpot : MonoBehaviour, IInteractable
{
    [SerializeField] Transform hidePoint;
    [SerializeField] Transform exitPoint;

    [SerializeField] Transform cameraPoint;
    [SerializeField] Transform cameraPivot;

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
        player.CurrentHideSpot = this; // 현재 숨은 위치 반환
        cc.enabled = false; // 물리 끄기
        player.transform.position = hidePoint.position;
        cameraPivot.SetParent(cameraPoint); // 숨는 위치로 카메라 연결
        cameraPivot.localPosition = Vector3.zero;
        cameraPivot.localRotation = Quaternion.identity;
        player.SetHidden(true); // 숨음
    }
    void Exit()
    {
        player.CurrentHideSpot = null; // 숨은 위치 null
        cc.enabled = false; // 이거 안하면 튕겨나감. 
        player.transform.position = exitPoint.position;
        cc.enabled = true; // 물리 켜기
        cameraPivot.transform.SetParent(player.transform); // 플레이어에게 다시 연결(위치 이동 전에 부모 복구부터 해야함)
        cameraPivot.localPosition = new Vector3(0f, 1.6f, 0f);
        cameraPivot.localRotation = Quaternion.identity;
        player.SetHidden(false); // 탈출
    }
}
