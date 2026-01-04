using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("生命值设置")]
    public float health = 50f; // 敌人总血量
    public int scoreReward = 100; // 击杀后玩家获得的得分

    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float patrolDistance = 5f; // 以出生点为中心的巡逻半径

    [Header("伤害设置")]
    public float damageAmount = 20f; // 撞击玩家造成的伤害

    [Header("重生设置")]
    public float fallThreshold = -10f; // 掉落重生的 y 轴高度

    private Vector3 startPosition;
    private int direction = 1; // 1 为右，-1 为左
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        // 锁定旋转，防止物理碰撞导致敌人翻滚
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // 1. 简单的左右巡逻逻辑
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // 如果超出巡逻范围，则转向
        if (Vector3.Distance(startPosition, transform.position) >= patrolDistance)
        {
            direction *= -1;
            // 翻转 Sprite 朝向
            transform.localScale = new Vector3(direction, 1, 1);
        }

        // 2. 敌人坠崖重生检测
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    // --- 核心接口：供玩家攻击脚本调用 ---
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " 受到伤害，剩余血量: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 尝试给玩家加分
        PlayerController player = GameObject.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddScore(scoreReward);
        }

        // 可以在这里播放死亡特效或音效
        Debug.Log(gameObject.name + " 已被击败");
        Destroy(gameObject);
    }

    // 碰撞检测：撞到玩家时触发
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }
        }
    }

    private void Respawn()
    {
        transform.position = startPosition;
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 重置物理速度
        }
    }
}