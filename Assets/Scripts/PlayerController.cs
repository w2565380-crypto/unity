using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    [Range(0, 1)] public float cutJumpModifier = 0.5f; // 短按跳跃时的力度削减

    [Header("地面检测")]
    public Transform groundCheck;     // 在玩家脚下放一个空物体作为检测点
    public float checkRadius = 0.2f;  // 检测半径
    public LayerMask groundLayer;     // 勾选哪些层是“地面”

    [Header("重生设置")]
    public float fallThreshold = -10f; // 掉落到 Y 轴此高度以下触发重生
    private Vector3 startPosition;    // 记录初始位置

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 1. 防止方块碰撞时乱滚
        rb.freezeRotation = true;

        // 2. 记录出生点
        startPosition = transform.position;

        // 自动容错：如果忘记拖拽 GroundCheck，尝试从子物体找
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
        }
    }

    void Update()
    {
        // 3. 获取输入 (GetAxisRaw 让操作没有延迟感)
        moveInput = Input.GetAxisRaw("Horizontal");

        // 4. 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 5. 跳跃处理
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
                canDoubleJump = false; // 用完二段跳
            }
        }

        // 6. 跳跃优化：松开跳跃键时减小向上速度（实现长按跳得高，短按跳得低）
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpModifier);
        }

        // 7. 角色转向 (翻转缩放)
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // 8. 死亡检测
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    void FixedUpdate()
    {
        // 9. 物理移动建议在 FixedUpdate 中执行
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        // 先重置 Y 轴速度，确保二段跳力度一致
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity = Vector2.up * jumpForce;
    }

    public void Respawn()
    {
        transform.position = startPosition;
        rb.velocity = Vector2.zero; // 重生时静止，防止动量累积
    }

    // 在编辑器里画出红色检测球，方便调试
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}