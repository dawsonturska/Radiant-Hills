using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Move speed for player")]
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private AudioClip movementClip;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;
    private Vector2 lastInputDirection;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = movementClip;
        audioSource.loop = true;
    }
    private void Start()
    {
        lastInputDirection = Vector2.down;
        SetToSpawnPosition();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetToSpawnPosition();
    }

    private void SetToSpawnPosition()
    {
        if (PlayerSpawnPoint.spawnPosition != Vector2.zero)
        {
            transform.position = PlayerSpawnPoint.spawnPosition;
        }
    }

    private void MovePlayer()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnimation()
    {
        if (moveDirection.sqrMagnitude > 0)
        {
            animator.SetFloat("MoveX", moveDirection.x);
            animator.SetFloat("MoveY", moveDirection.y);
            lastInputDirection = moveDirection;
        }

        animator.SetFloat("IdleX", lastInputDirection.x);
        animator.SetFloat("IdleY", lastInputDirection.y);
        animator.SetBool("IsMoving", moveDirection.sqrMagnitude > 0);
    }

    private void HandleMovementAudio()
    {
        if (moveDirection.sqrMagnitude > 0)
        {
            if (!audioSource.isPlaying && movementClip != null)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    /// <summary>
    /// Handle player movement given context.performed and context.canceled
    /// </summary>
    /// <param name="context"></param>
    public void HandleMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveDirection = Vector2.zero;
        }

        // Update Animator parameters
        UpdateAnimation();

        if (moveDirection != Vector2.zero)
        {
            lastInputDirection = moveDirection.normalized;
        }

        HandleMovementAudio();

        // if we want to add diagonal animations, just add more sprites in the blend tree for these values:
        // (1, 1), (-1, 1), (1, -1), (-1, -1)
    }

    /// <summary>
    /// Return moveDirection so other objects can reference it
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveDirection() { return moveDirection; }
}