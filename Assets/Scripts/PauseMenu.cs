using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // 是否处于暂停状态
    public static bool isPaused = false;

    // 暂停菜单面板物体
    public GameObject pauseMenuUI;

    void Update()
    {
        // 监听 ESC 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // 继续游戏
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // 隐藏面板
        Time.timeScale = 1f;          // 恢复正常游戏速度
        isPaused = false;
    }

    // 暂停游戏
    void Pause()
    {
        pauseMenuUI.SetActive(true);  // 显示面板
        Time.timeScale = 0f;          // 冻结游戏时间（物理和动画都会停止）
        isPaused = true;
    }

    // 回到主菜单
    public void LoadMenu()
    {
        Time.timeScale = 1f;          // 非常重要：回到菜单前必须恢复时间，否则菜单也是卡死的
        SceneManager.LoadScene("Start"); // 替换为你主菜单场景的名字
    }
}