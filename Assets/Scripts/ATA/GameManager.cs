using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public MeshRenderer leftLegRenderer;
    public MeshRenderer rightLegRenderer;
    public bool canPlayerMove = true;
    private Material bossLegLeftMaterial;
    private Material bossLegRightMaterial;
    public Animator playerAnimator;
    void Awake()
    {
        bossLegLeftMaterial = leftLegRenderer.material;
        bossLegRightMaterial = rightLegRenderer.material;

        canPlayerMove = true;
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
        playerAnimator.SetTrigger("PlayerBossMutation");
        canPlayerMove = false;
        ManagerCinemachine.Instance.SetBossMutationCamera();
        DOTween.To(() => bossLegLeftMaterial.GetFloat("_DissolveAmount"),
                       x => bossLegLeftMaterial.SetFloat("_DissolveAmount", x),
                       0f,
                       1.7f).SetEase(Ease.OutSine);

        DOTween.To(() => bossLegRightMaterial.GetFloat("_DissolveAmount"),
                       x => bossLegRightMaterial.SetFloat("_DissolveAmount", x),
                       0f,
                       1.7f).SetEase(Ease.OutSine);
    }



}
