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
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim; // 动画控制器引用
    private bool isGrounded;
    private bool canDoubleJump;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // 获取动画组件

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
        // 1. 移动输入与地面检测
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

        // 短跳优化
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
            Respawn();

        // 6. 核心：更新动画参数（传给动画师）
        UpdateAnimations();
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

    private void UpdateAnimations()
    {
        if (anim != null)
        {
            // speed: 大于0.1时切换跑步动画
            anim.SetFloat("speed", Mathf.Abs(moveInput));

            // grounded: 为false时播放跳跃/浮空动画
            anim.SetBool("grounded", isGrounded);

            // vSpeed: y轴垂直速度，用于判断是上升还是下落
            anim.SetFloat("vSpeed", rb.velocity.y);
        }
    }

    // --- 外部接口 ---

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        sr.color = new Color(1, 1, 1, 0.5f);

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




