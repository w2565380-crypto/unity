using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10; // 该食物的分值

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 调用玩家身上的加分接口
            collision.GetComponent<PlayerController>().AddScore(scoreValue);

            // 吃掉后物体消失
            Destroy(gameObject);
        }
    }
}