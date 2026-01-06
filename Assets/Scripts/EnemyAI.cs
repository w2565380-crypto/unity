using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("生命值设置")]
    public float health = 50f;
    public int scoreReward = 100;

    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float patrolDistance = 5f; // 巡逻半径

    [Header("伤害设置")]
    public float damageAmount = 20f;

    [Header("重生设置")]
    public float fallThreshold = -10f;

    private Vector3 startPosition;
    private float leftBoundary;
    private float rightBoundary;
    private int direction = 1; // 1 为右，-1 为左

    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 记录出生点并计算固定的左右边界坐标
        startPosition = transform.position;
        leftBoundary = startPosition.x - patrolDistance;
        rightBoundary = startPosition.x + patrolDistance;

        // 锁定旋转
        if (rb != null)
        {
            rb.freezeRotation = true;
            // 建议在 Inspector 将 Rigidbody2D 的 Body Type 设为 Kinematic
            // 如果必须用 Dynamic，请取消下面这行的注释来增加重量感
            // rb.mass = 100f; 
        }
    }

    void Update()
    {
        if (isDead) return;

        // 1. 巡逻逻辑：基于固定坐标边界，防止抽搐
        Move();

        // 2. 坠崖检测
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    private void Move()
    {
        // 改用世界坐标系下的位移，这样不受物体自身旋转(Flip)的影响
        // direction 为 1 时向右偏移，为 -1 时向左偏移
        transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

        // 检查是否到达右边界
        if (direction > 0 && transform.position.x >= rightBoundary)
        {
            direction = -1;
            Flip();
        }
        // 检查是否到达左边界
        else if (direction < 0 && transform.position.x <= leftBoundary)
        {
            direction = 1;
            Flip();
        }
    }

    private void Flip()
    {
        // 旋转物体 Y 轴来实现视觉上的翻转
        if (direction == 1)
            transform.eulerAngles = Vector3.zero;
        else
            transform.eulerAngles = new Vector3(0, 180, 0);
    }

    // --- 核心接口：供玩家攻击脚本调用 ---
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log(gameObject.name + " 受到伤害，剩余血量: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // 触发死亡动画
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        // 给玩家加分
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddScore(scoreReward);
        }

        // 禁用碰撞体，防止死后还能挡路或伤人
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log(gameObject.name + " 已被击败");

        // 延迟销毁，给死亡动画留时间
        Destroy(gameObject, 1.0f);
    }

    // 碰撞检测：撞到玩家时触发
    // 注意：如果在 Physics2D 设置里取消了 Player 和 Enemy 的碰撞硬阻挡，
    // 请确保 Collider 勾选了 Is Trigger，并将此函数改为 OnTriggerEnter2D
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerContact(collision.gameObject);
    }

    // 确保这个函数存在，因为你勾选了 Is Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                Debug.Log("撞到玩家，造成伤害！"); // 运行测试时看一眼 Console 面板
            }
        }
    }

    private void HandlePlayerContact(GameObject other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }

    private void Respawn()
    {
        transform.position = startPosition;
        health = 50f; // 重置血量
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
}