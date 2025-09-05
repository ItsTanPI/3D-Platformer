using UnityEngine;

public class SpinningHazard : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageCooldown = 1f; 

    [Header("Spin Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up; 
    [SerializeField] private float rotationSpeed = 90f;

    private float lastDamageTime = 0f;

    private void Update()
    {

        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerStay(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && Time.time >= lastDamageTime + damageCooldown)
        {
            health.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}
