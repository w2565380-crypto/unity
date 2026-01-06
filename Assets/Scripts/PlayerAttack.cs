using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackDamage = 20f;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    private Animator anim; // 引用动画组件

    void Start()
    {
        // 获取挂在同一个物体（Player）上的 Animator
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // 建议使用 Input.GetButtonDown("Fire1") 兼容鼠标左键，或者保持 KeyCode.J
            if (Input.GetKeyDown(KeyCode.J))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. 触发动画接口
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        // 2. 检测攻击范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. 对受击对象造成伤害
        foreach (Collider2D enemy in hitEnemies)
        {
            // 增加空引用检查，防止报错
            var enemyComponent = enemy.GetComponent<EnemyAI>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red; // 给调试圆圈加个颜色
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}