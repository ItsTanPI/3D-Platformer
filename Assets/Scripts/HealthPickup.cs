using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Health, Coin }

    [Header("Pickup Settings")]
    [SerializeField] private PickupType type = PickupType.Health;
    [SerializeField] private int amount = 10;

    [Header("Visual Effects")]
    [SerializeField] private float hoverHeight = 0.25f;   
    [SerializeField] private float hoverSpeed = 2f;       
    [SerializeField] private float rotationSpeed = 45f;   

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newZ = startPos.z + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (type)
        {
            case PickupType.Health:
                Health health = other.GetComponent<Health>();
                if (health != null)
                {
                    health.Heal(amount);
                    Destroy(gameObject);
                }
                break;

            case PickupType.Coin:
                Score score = other.GetComponent<Score>();
                if (score != null)
                {
                    score.UpdateScore(amount);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
