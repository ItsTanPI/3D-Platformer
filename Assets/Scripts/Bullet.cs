using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int _damage = 20;
    [SerializeField] float _lifeTime = 5f;
    [SerializeField] float _force = 5.0f;
    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(_damage);
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            Vector3 forceDir = transform.forward;

            rb.AddForceAtPosition(forceDir * _force, hitPoint, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
