using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用这个命名空间

public class FinishLine : MonoBehaviour
{
    [Header("设置")]
    public string nextSceneName; // 下一个关卡的名称
    public float delayTime = 1.5f; // 切换前的延迟时间（可以播放特效）

    private bool isFinished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 确保是玩家触碰，且还没触发过
        if (collision.CompareTag("Player") && !isFinished)
        {
            isFinished = true;
            Debug.Log("到达终点！准备进入下一关：" + nextSceneName);

            // 延迟加载，给玩家一点反馈时间
            Invoke("LoadNextLevel", delayTime);
        }
    }

    void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("未设置下一个场景的名字！");
        }
    }
}