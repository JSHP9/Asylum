using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isLocked = true;

    public void Interact(GameObject interactor)
    {
        if (isLocked)
        {
            Debug.Log("문이 잠겨있음.");
            return;
        }
        SceneManager.LoadScene("Gameover_page");
    }
        

    public void UnlockDoor()
    {
        isLocked = false;
    }
}
