using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    
    public bool canPlayerMove = true;
    void Awake()
    {
     
        Application.targetFrameRate = 60;

        QualitySettings.vSyncCount = 0;
    }
    public void RestartTheLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void IntroNarrativeSequence()
    {

    }
    public void FirstBossNarrativeSequence()
    {

    }
    public void FinalBossNarrativeSequence()
    {

    }

    public void OnBossDefeated()
    {
        ManagerSave.Instance.SaveState.isFirstBossDefeated = true;
        ManagerSave.Instance.Save();

    }



}
