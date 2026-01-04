using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint; // 攻击中心点（在玩家前方的一个空物体）
    public float attackRange = 0.5f; // 攻击半径
    public LayerMask enemyLayers; // 设置为 Enemy 层
    public float attackDamage = 20f;
    public float attackRate = 2f; // 每秒攻击次数
    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.J)) // 假设按 J 键攻击
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. 检测攻击范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 2. 对受击对象造成伤害
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAI>().TakeDamage(attackDamage);
        }
    }

    // 在编辑器里画出攻击范围，方便调试
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}