using UnityEditor.Rendering.LookDev;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 6f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float stopDistance = 2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float edgeCheckDistance = 1.0f;
    [SerializeField] private float obstacleCheckDistance = 1.0f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Combat")]
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    private CharacterController controller;
    private float currentSpeed = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        Vector3 moveDir = Vector3.zero;

        if (toPlayer.magnitude > stopDistance)
        {
            Vector3 edgeOrigin = transform.position + transform.forward * edgeCheckDistance;
            bool groundAhead = Physics.Raycast(edgeOrigin, Vector3.down, 2f, groundMask);

            bool isObstacleAhead = Physics.Raycast(transform.position + Vector3.up * -0.25f, transform.forward, obstacleCheckDistance, groundMask);

            if (!groundAhead)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * deceleration);
            }
            else if (isObstacleAhead)
            {
                if (controller.isGrounded)
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * acceleration);
                moveDir = toPlayer.normalized;
            }

        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * deceleration);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (animator) animator.SetTrigger("Hit");

                var health = player.GetComponent<Health>();
                if (health)
                {
                    float yDiff = Mathf.Abs(player.position.y - transform.position.y);
                    if (yDiff <= 0.5f) health.TakeDamage(10);
                }

                lastAttackTime = Time.time;
            }
        }

        Vector3 velocity = moveDir * currentSpeed;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        Vector3 lookDir = moveDir;

        if (lookDir.sqrMagnitude < 0.01f)
        {
            lookDir = (player.position - transform.position);
        }

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }

        if (animator) animator.SetFloat("Speed", currentSpeed / maxSpeed);


        if(transform.position.y < -10) GetComponent<Health>().TakeDamage(-1);
     }

    private void OnDestroy()
    {
        player.GetComponent<Score>().UpdateScore(10);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 edgeOrigin = transform.position + transform.forward * edgeCheckDistance;
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector3.down * 2f);

        Gizmos.color = Color.blue;
        Vector3 obstacleStart = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(obstacleStart, obstacleStart + transform.forward * obstacleCheckDistance);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
