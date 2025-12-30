using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public float damageAmount = 10f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 如果碰到的是玩家，且玩家身上有这个脚本
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(damageAmount * Time.deltaTime);
        }
    }
}