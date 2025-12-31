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
    public float invincibilityDuration = 1f; // 1秒无敌时间
    private float invincibilityTimer;
    private bool isInvincible;

    [Header("得分设置")]
    public int score = 0;
    public Text scoreText;

    [Header("重生设置")]
    public float fallThreshold = -10f;
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isGrounded;
    private bool canDoubleJump;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
        // 1. 移动输入
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 2. 跳跃逻辑（含二段跳）
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

        // 长按跳得高，短按跳得低
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpModifier);

        // 3. 翻转朝向
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // 4. 处理无敌计时
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                sr.color = Color.white; // 恢复不透明
            }
        }

        // 5. 坠崖检测
        if (transform.position.y < fallThreshold)
            Respawn();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity = Vector2.up * jumpForce;
    }

    // --- 核心机制接口 ---

    public void TakeDamage(float damage)
    {
        if (isInvincible) return; // 无敌中不扣血

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        // 开启无敌效果
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        sr.color = new Color(1, 1, 1, 0.5f); // 变半透明

        if (currentHealth <= 0) Respawn();
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
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        currentHealth = maxHealth;
        isInvincible = false;
        sr.color = Color.white;
        UpdateHealthUI();
    }
}