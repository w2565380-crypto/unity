using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [Header("设置")]
    public string nextSceneName = "MenuScene"; // 默认设为你的主菜单场景名
    public float delayTime = 1.5f;

    private bool isFinished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFinished)
        {
            isFinished = true;
            Debug.Log("到达终点！即将返回关卡选择界面");

            // --- 核心逻辑：设置返回主菜单后自动打开 LevelPanel ---
            MainMenu.showLevelSelectOnStart = true;

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
            // 如果你忘记填名字，默认回主菜单
            SceneManager.LoadScene("MenuScene");
        }
    }
}