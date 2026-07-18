using UnityEngine;

public class ElectricWireGroup : MonoBehaviour
{
    // 퓨즈 박스 Unity Event에서 호출 하는 함수
    public void RemoveWire()
    {
        gameObject.SetActive(false);
    }

}
