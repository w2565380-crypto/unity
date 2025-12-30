using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("地面检测")]
    public Transform groundCheck;     // 在玩家脚下放一个空物体作为检测点
    public float checkRadius = 0.2f;  // 检测半径
    public LayerMask groundLayer;     // 勾选哪些层是“地面”

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 防止方块在碰撞时乱滚
        rb.freezeRotation = true;
        // 建议在 Inspector 面板将 Collision Detection 改为 Continuous
    }

    void Update()
    {
        // 1. 获取输入
        moveInput = Input.GetAxisRaw("Horizontal"); // GetAxisRaw 让移动更灵敏，没有惯性

        // 2. 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 3. 跳跃处理
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 4. 转向处理 (可选：让角色根据移动方向左右翻转)
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // 5. 物理移动建议在 FixedUpdate 中处理
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    // 在编辑器里画出检测范围，方便调试
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}