using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Scene Names")]
    public string gameSceneName = "Game";   // change to actual game scene name

    void Start()
    {
        ShowMainMenu();
    }

    // ---------- Panel switching ----------
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void CloseOptions()
    {
        ShowMainMenu();
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        ShowMainMenu();
    }

    // ---------- Buttons ----------
    public void PlayNewGame()
    {
        
        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit called (won't close in editor)");
    }
}
