using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public float damageAmount = 20f; // 每秒造成的伤害值

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // 注意：这里每秒扣除 damageAmount 点血
                player.TakeDamage(damageAmount * Time.deltaTime);
            }
        }
    }
}