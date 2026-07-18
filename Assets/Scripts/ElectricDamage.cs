using UnityEngine;

public class ElectricDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("감전사");
        }
    }
}