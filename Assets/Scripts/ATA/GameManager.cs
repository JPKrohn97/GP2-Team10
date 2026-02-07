using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager>
{
    public MeshRenderer leftLegRenderer;
    public MeshRenderer rightLegRenderer;
    public bool canPlayerMove = true;
    private Material bossLegLeftMaterial;
    private Material bossLegRightMaterial;
    public Animator playerAnimator;
    public Image blackFadeImage;
    private PlayerController playerController;
    void Awake()
    {
        bossLegLeftMaterial = leftLegRenderer.material;
        bossLegRightMaterial = rightLegRenderer.material;

        canPlayerMove = true;
        Application.targetFrameRate = 60;

        QualitySettings.vSyncCount = 0;
    }
    public void CinematicBlackFadeIn(float speed)
    {
        blackFadeImage.DOFade(1f, speed).SetEase(Ease.InOutSine);
    }
    public void CinematicBlackFadeOut(float speed)
    {
        blackFadeImage.DOFade(0f, speed).SetEase(Ease.InOutSine);
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
        canPlayerMove = false;

        DOVirtual.DelayedCall(1.5f, () =>
        {
            playerAnimator.SetTrigger("PlayerBossDefeated");
             ManagerCinemachine.Instance.SetBossMutationCamera();
        });
    }

    public void BossMutationSequqnce()
    {
        ManagerSave.Instance.SaveState.isFirstBossDefeated = true;
        ManagerSave.Instance.Save();
        playerAnimator.SetTrigger("PlayerBossMutation");
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
