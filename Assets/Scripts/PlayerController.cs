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
    public float deathDelay = 1.5f; // 给死亡动画留出的时间
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private bool isGrounded;
    private bool canDoubleJump;
    private float moveInput;
    private bool isDead = false; // 标记是否已死亡

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
        if (isDead) return; // 死亡期间禁止任何输入控制

        // 1. 移动输入与地面检测
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

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
                sr.color = Color.white;
            }
        }

        // 5. 坠崖检测
        if (transform.position.y < fallThreshold)
            Die();

        // 6. 更新动画参数
        UpdateAnimations();
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

    // --- 外部接口 ---

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

            // 如果有受击动画，可以取消下面注释
            // if(anim != null) anim.SetTrigger("hurt");
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        moveInput = 0;
        rb.velocity = Vector2.zero;
        rb.simulated = false; // 死亡时关闭物理模拟，防止尸体乱动

        if (anim != null)
        {
            anim.SetTrigger("die"); // 触发死亡动画
        }

        // 延迟调用复活逻辑
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
        UpdateHealthUI();

        if (anim != null)
        {
            anim.SetTrigger("respawn"); // 触发复活/出生动画
            anim.Rebind(); // 强制重置动画状态机
        }
    }
}