using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("面板引用")]
    public GameObject mainPanel;   // 主菜单面板 (含有 Start, Exit 按钮)
    public GameObject levelPanel;  // 关卡选择面板 (含有 Level1, Level2, Back 按钮)

    // --- 主菜单逻辑 ---

    public void OpenLevelSelect()
    {
        // 切换面板显示
        mainPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("游戏已退出");
        Application.Quit();
    }

    // --- 关卡选择逻辑 ---

    public void LoadLevel1()
    {
        // 假设第一关场景名为 "Level1"
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        // 假设第二关场景名为 "Level2"
        SceneManager.LoadScene("Level2");
    }

    public void GoBack()
    {
        // 返回主菜单面板
        levelPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}