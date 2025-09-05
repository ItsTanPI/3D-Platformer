using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;  
    [SerializeField] private string targetTag = "Player";  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Destroy(gameObject);
        }
    }
}
