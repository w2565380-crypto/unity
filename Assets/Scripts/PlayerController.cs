using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    [Range(0, 1)] public float cutJumpModifier = 0.5f;

    [Header("地面检测")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("生命值设置")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;

    [Header("无敌设置")]
    public float invincibilityDuration = 1f;
    private float invincibilityTimer;
    private bool isInvincible;

    [Header("得分设置")]
    public int score = 0;
    public Text scoreText;

    [Header("重生设置")]
    public float fallThreshold = -10f;
    public float deathDelay = 1.5f;
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool isGrounded;
    private bool canDoubleJump;
    private float moveInput;
    private bool isDead = false;

    // --- 新增：用于锁定朝向的变量 ---
    private float lastFacingDir = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        rb.freezeRotation = true;
        startPosition = transform.position;
        currentHealth = maxHealth;

        UpdateHealthUI();
        UpdateScoreUI();

        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        if (isDead) return;

        // 1. 移动输入与地面检测
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // --- 核心修改：更新记录的方向 ---
        if (moveInput > 0) lastFacingDir = 1f;
        else if (moveInput < 0) lastFacingDir = -1f;

        // 2. 跳跃逻辑
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpModifier);

        // 3. 处理无敌计时
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                sr.color = Color.white;
            }
        }

        // 4. 坠崖检测
        if (transform.position.y < fallThreshold)
            Die();

        // 5. 更新动画参数
        UpdateAnimations();
    }

    // --- 核心修改：在 LateUpdate 中强制锁定缩放 ---
    // LateUpdate 在所有动画计算之后执行，可以确保覆盖 Animator 的自动重置
    void LateUpdate()
    {
        if (isDead) return;
        transform.localScale = new Vector3(lastFacingDir, 1f, 1f);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity = Vector2.up * jumpForce;
    }

    private void UpdateAnimations()
    {
        if (anim != null)
        {
            anim.SetFloat("speed", Mathf.Abs(moveInput));
            anim.SetBool("grounded", isGrounded);
            anim.SetFloat("vSpeed", rb.velocity.y);
            anim.SetFloat("health", currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            sr.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        moveInput = 0;
        rb.velocity = Vector2.zero;
        rb.simulated = false;

        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        Invoke("Respawn", deathDelay);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateHealthUI() { if (healthSlider != null) healthSlider.value = currentHealth / maxHealth; }
    void UpdateScoreUI() { if (scoreText != null) scoreText.text = "Score: " + score; }

    public void Respawn()
    {
        isDead = false;
        rb.simulated = true;
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        currentHealth = maxHealth;
        isInvincible = false;
        sr.color = Color.white;
        lastFacingDir = 1f; // 重生时默认面向右
        UpdateHealthUI();

        if (anim != null)
        {
            anim.SetTrigger("respawn");
            anim.Rebind();
        }
    }
}