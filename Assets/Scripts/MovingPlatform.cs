using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("路径设置")]
    public Transform posA; // 起点
    public Transform posB; // 终点
    public float speed = 3f;

    private Vector3 targetPos;

    void Start()
    {
        // 初始目标设为 B 点
        targetPos = posB.position;
    }

    void Update()
    {
        // 1. 往返逻辑：到达目标点后切换目标
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            targetPos = (targetPos == posA.position) ? posB.position : posA.position;
        }

        // 2. 移动平台
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    // --- 核心：解决角色站不稳/不跟随的问题 ---
    // 当玩家踩上平台时，将平台设为玩家的父物体
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 只有当玩家和平台都在激活状态时才操作
            if (gameObject.activeInHierarchy && collision.gameObject.activeInHierarchy)
            {
                collision.gameObject.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 增加判断：如果玩家正在被销毁或者平台正在停用，不强制在此时解除父子关系
            // 而是由 Unity 系统自动处理或直接设为 null
            if (gameObject.activeInHierarchy)
            {
                collision.gameObject.transform.SetParent(null);
            }
            else
            {
                // 如果平台失效了，直接把玩家移出层级而不通过 SetParent 触发报错
                collision.gameObject.transform.parent = null;
            }
        }
    }
}