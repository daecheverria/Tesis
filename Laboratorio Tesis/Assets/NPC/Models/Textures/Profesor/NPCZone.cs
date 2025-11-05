using UnityEngine;
using System.Collections;

public class NPCZone : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTimeAtPoints = 1f;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Punto especial")]
    [Tooltip("Índice del punto donde el NPC se detiene 20 segundos (0 = primer punto)")]
    [SerializeField] private int specialPointIndex = 3;
    [Tooltip("Tiempo de espera en el punto especial (segundos)")]
    [SerializeField] private float specialWaitTime = 20f;

    private Rigidbody rb;
    private int currentPatrolIndex = 0;
    private bool isWaiting = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || isWaiting) return;

        Transform point = patrolPoints[currentPatrolIndex];
        if (point == null) return;

        Vector3 targetPos = point.position;
        targetPos.y = transform.position.y;

        Vector3 direction = (targetPos - transform.position);
        direction.y = 0;
        direction.Normalize();

        rb.MovePosition(transform.position + direction * patrolSpeed * Time.fixedDeltaTime);

        if (direction != Vector3.zero)
            transform.forward = direction;

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist < 0.2f)
        {
            // Si es el punto especial, usa el Animator y espera el tiempo especial
            if (currentPatrolIndex == specialPointIndex && animator != null)
            {
                animator.SetBool("isParado", true);
                animator.SetBool("isSeguir", false);
                StartCoroutine(WaitAndNextPoint(specialWaitTime, true));
            }
            else
            {
                animator?.SetBool("isParado", false);
                animator?.SetBool("isSeguir", true);
                StartCoroutine(WaitAndNextPoint(waitTimeAtPoints, false));
            }
        }
    }

    private IEnumerator WaitAndNextPoint(float waitTime, bool wasSpecial)
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        // Si estaba en el punto especial, reanuda la animación de caminar
        if (wasSpecial && animator != null)
        {
            animator.SetBool("isParado", false);
            animator.SetBool("isSeguir", true);
        }

        isWaiting = false;
    }
}