using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
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


}
