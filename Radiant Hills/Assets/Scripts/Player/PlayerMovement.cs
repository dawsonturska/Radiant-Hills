using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;
    private Vector2 lastInputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastInputDirection = Vector2.down;

        SetToSpawnPosition();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    void Update()
    {
        HandleInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveY != 0)
        {
            moveDirection = new Vector2(0, moveY);
            lastInputDirection = moveDirection;
            animator.SetTrigger(moveY > 0 ? "MoveUp" : "MoveDown");
            animator.SetTrigger("StartMoving");
        }
        else if (moveX != 0)
        {
            moveDirection = new Vector2(moveX, 0);
            lastInputDirection = moveDirection;
            animator.SetTrigger(moveX > 0 ? "MoveRight" : "MoveLeft");
            animator.SetTrigger("StartMoving");
        }
        else
        {
            moveDirection = Vector2.zero;
            animator.ResetTrigger("StartMoving");
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
}
