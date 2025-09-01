using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _lifeTime = 5f;

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

        Destroy(gameObject);
    }
}
