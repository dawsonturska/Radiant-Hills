using UnityEngine;
using System.Collections;

public class Burrower : Enemy
{
    public float burrowCooldown = 3f;
    private float burrowCooldownTimer = 0f;
    private bool isBurrowing = false;

    public Collider2D burrowZone;
    private LineRenderer lineRenderer;

    public float damageDelay = 1f;
    public int damageAmount = 10;
    public float damageCooldown = 3f;
    private float damageCooldownTimer = 0f;

    public float burrowDelay = 1f;

    public float knockbackForce = 5f;
    public float knockbackDuration = 0.5f;
    private bool isKnockedBack = false;
    private Vector2 knockbackDirection;

    public float maxBurrowDistance = 10f;

    new void Start()
    {
        base.Start();

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.positionCount = 0;
    }

    new void Update()
    {
        base.Update();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 velocity = rb.velocity;
        bool isMoving = velocity.magnitude > 0.1f;

        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            Vector2 dir = velocity.normalized;
            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
        }

        if (isInAggroRange && !isBurrowing)
        {
            burrowCooldownTimer += Time.deltaTime;

            if (burrowCooldownTimer >= burrowCooldown)
            {
                StartCoroutine(BurrowToRandomPoint());
                burrowCooldownTimer = 0f;
            }
        }

        if (damageCooldownTimer > 0f)
        {
            damageCooldownTimer -= Time.deltaTime;
        }

        if (isKnockedBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + knockbackDirection, knockbackForce * Time.deltaTime);
            knockbackDuration -= Time.deltaTime;

            if (knockbackDuration <= 0)
            {
                isKnockedBack = false;
            }
        }

        if (!isBurrowing)
        {
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            animator.SetFloat("MoveX", playerDirection.x);
            animator.SetFloat("MoveY", playerDirection.y);
        }
    }

    private IEnumerator BurrowToRandomPoint()
    {
        isBurrowing = true;

        Vector3 oldPosition = transform.position;
        Vector3 burrowTarget = GetRandomBurrowPoint(oldPosition);

        if (Vector3.Distance(oldPosition, burrowTarget) <= maxBurrowDistance)
        {
            if (burrowTarget != Vector3.zero && IsPathClear(burrowTarget))
            {
                Debug.Log("Burrower burrowing to: " + burrowTarget);

                animator.SetTrigger("Retreat");
                yield return new WaitForSeconds(0.5f);

                DrawBurrowTrail(oldPosition, burrowTarget);

                float timeToMove = 0.5f;
                float elapsedTime = 0f;

                Vector3 burrowDirection = (burrowTarget - oldPosition).normalized;
                animator.SetFloat("MoveX", burrowDirection.x);
                animator.SetFloat("MoveY", burrowDirection.y);
                animator.SetBool("IsMoving", true);

                while (elapsedTime < timeToMove)
                {
                    transform.position = Vector3.Lerp(oldPosition, burrowTarget, elapsedTime / timeToMove);
                    elapsedTime += Time.deltaTime;

                    animator.SetFloat("MoveX", burrowDirection.x);
                    animator.SetFloat("MoveY", burrowDirection.y);

                    yield return null;
                }

                animator.SetBool("IsMoving", false);
                transform.position = burrowTarget;

                animator.SetTrigger("Popup");
                yield return new WaitForSeconds(0.5f);

                Debug.Log("Burrower moved to burrow position: " + burrowTarget);
            }
            else
            {
                Debug.Log("Burrow path blocked, retrying.");
            }
        }
        else
        {
            Debug.Log("Target burrow point is too far, retrying.");
        }

        isBurrowing = false;
    }

    private Vector3 GetRandomBurrowPoint(Vector3 currentPosition)
    {
        if (burrowZone != null)
        {
            Bounds bounds = burrowZone.bounds;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            Vector3 randomPoint = new Vector3(randomX, randomY, currentPosition.z);

            if (Vector3.Distance(currentPosition, randomPoint) <= maxBurrowDistance)
            {
                return randomPoint;
            }
            else
            {
                Vector3 direction = (randomPoint - currentPosition).normalized;
                return currentPosition + direction * maxBurrowDistance;
            }
        }

        Debug.LogWarning("Burrow zone is not set in the inspector!");
        return currentPosition;
    }

    private bool IsPathClear(Vector3 targetPoint)
    {
        LayerMask objectsLayerMask = LayerMask.GetMask("Objects");

        Vector2 direction = targetPoint - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, objectsLayerMask);
        Debug.DrawRay(transform.position, direction.normalized * distance, Color.red, 1f);

        if (hit.collider != null)
        {
            Debug.Log($"Path blocked by: {hit.collider.name}");
            return false;
        }

        return true;
    }

    private void DrawBurrowTrail(Vector3 startPoint, Vector3 endPoint)
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
            StartCoroutine(HideTrailAfterDelay(0.5f));
        }
        else
        {
            Debug.LogWarning("LineRenderer is not assigned or initialized!");
        }
    }

    private IEnumerator HideTrailAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
