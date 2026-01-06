using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("面板引用")]
    public GameObject mainPanel;   // 主菜单面板 (含有 Start, Exit 按钮)
    public GameObject levelPanel;  // 关卡选择面板 (含有 Level1, Level2, Back 按钮)

    // 静态变量：用于记录是否需要直接打开关卡选择页（通关后使用）
    public static bool showLevelSelectOnStart = false;

    void Start()
    {
        // 逻辑：每次回到主菜单场景时，检查是否是从关卡成功返回的
        if (showLevelSelectOnStart)
        {
            mainPanel.SetActive(false);
            levelPanel.SetActive(true);
            showLevelSelectOnStart = false; // 及时重置，防止下次点开游戏还是这个界面
        }
        // 情况 B：正常启动游戏，确保只显示主面板
        else
        {
            mainPanel.SetActive(true);
            levelPanel.SetActive(false);
        }
    }

    // --- 主菜单逻辑 ---

    public void OpenLevelSelect()
    {
        mainPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("游戏已退出");
        Application.Quit(); // 退出程序
    }

    // --- 关卡选择逻辑 ---

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void GoBack()
    {
        levelPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}