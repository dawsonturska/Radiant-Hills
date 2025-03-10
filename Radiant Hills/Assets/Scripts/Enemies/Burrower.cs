using UnityEngine;
using System.Collections;


public class Burrower : Enemy
{
    public float burrowCooldown = 3f; // Cooldown time between each burrow
    private float burrowCooldownTimer = 0f; // Timer to manage burrow cooldown
    private bool isBurrowing = false; // Flag to check if the Burrower is currently burrowing

    public Collider2D burrowZone; // Reference to the burrow zone (set in the inspector)
    private LineRenderer lineRenderer;

    public float damageDelay = 1f; // Delay before damage is applied after collision
    public int damageAmount = 10; // Amount of damage to apply when hitting the player
    public float damageCooldown = 3f; // Cooldown for the damage to be applied again
    private float damageCooldownTimer = 0f; // Timer to manage the damage cooldown

    public float burrowDelay = 1f; // Delay after burrowing before the Burrower can burrow again

    // Knockback variables
    public float knockbackForce = 5f; // Force applied when taking damage
    public float knockbackDuration = 0.5f; // Duration of the knockback effect
    private bool isKnockedBack = false; // Flag to check if the Burrower is being knocked back
    private Vector2 knockbackDirection; // Direction to apply knockback

    // Burrow distance limit
    public float maxBurrowDistance = 10f; // Maximum distance the Burrower can move when burrowing

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        // Initialize LineRenderer
        lineRenderer = gameObject.GetComponent<LineRenderer>();
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

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (isInAggroRange && !isBurrowing)
        {
            burrowCooldownTimer += Time.deltaTime;

            // Start burrowing if the cooldown is over
            if (burrowCooldownTimer >= burrowCooldown)
            {
                StartCoroutine(BurrowToRandomPoint());
                burrowCooldownTimer = 0f;
            }
        }

        // Update damage cooldown timer
        if (damageCooldownTimer > 0f)
        {
            damageCooldownTimer -= Time.deltaTime;
        }

        // Handle knockback
        if (isKnockedBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + knockbackDirection, knockbackForce * Time.deltaTime);
            knockbackDuration -= Time.deltaTime;

            if (knockbackDuration <= 0)
            {
                isKnockedBack = false;
            }
        }
    }

    // Coroutine to handle the burrowing behavior
    private IEnumerator BurrowToRandomPoint()
    {
        isBurrowing = true;

        // Store the current position before burrowing
        Vector3 oldPosition = transform.position;

        // Find a random point inside the burrow zone, but constrained by maxBurrowDistance
        Vector3 burrowTarget = GetRandomBurrowPoint(oldPosition);

        if (Vector3.Distance(oldPosition, burrowTarget) <= maxBurrowDistance)
        {
            if (burrowTarget != Vector3.zero && IsPathClear(burrowTarget))
            {
                Debug.Log("Burrower burrowing to: " + burrowTarget);
                DrawBurrowTrail(oldPosition, burrowTarget);

                float timeToMove = 0.5f;
                float elapsedTime = 0f;

                while (elapsedTime < timeToMove)
                {
                    transform.position = Vector3.Lerp(oldPosition, burrowTarget, elapsedTime / timeToMove);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.position = burrowTarget;
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

    // Function to get a random point inside the burrow zone, considering maxBurrowDistance
    private Vector3 GetRandomBurrowPoint(Vector3 currentPosition)
    {
        if (burrowZone != null)
        {
            Bounds bounds = burrowZone.bounds;

            // Get random X and Y coordinates within the bounds of the burrow zone
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            Vector3 randomPoint = new Vector3(randomX, randomY, currentPosition.z);

            // Ensure the point is within maxBurrowDistance from the current position
            if (Vector3.Distance(currentPosition, randomPoint) <= maxBurrowDistance)
            {
                return randomPoint;
            }
            else
            {
                // If the random point is too far, pick a point closer to the Burrower's current position
                Vector3 direction = (randomPoint - currentPosition).normalized;
                return currentPosition + direction * maxBurrowDistance;
            }
        }

        Debug.LogWarning("Burrow zone is not set in the inspector!");
        return currentPosition;
    }

    // Function to check if the path to the target point is clear (excluding objects on "Objects" layer)
    private bool IsPathClear(Vector3 targetPoint)
    {
        LayerMask objectsLayerMask = LayerMask.GetMask("Objects");

        Vector2 direction = targetPoint - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, objectsLayerMask);
        Debug.DrawRay(transform.position, direction.normalized * distance, Color.red, 1f);
        Debug.Log($"Checking path to {targetPoint}, hit: {(hit.collider != null ? hit.collider.name : "Nothing")}");

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
