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
    public void OnBossDefeated()
    {
        canPlayerMove = false;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            CinematicBlackFadeIn(0.5f);
        });
        DOVirtual.DelayedCall(0.8f, () =>
        {
            playerAnimator.SetTrigger("PlayerBossDefeated");
            ManagerCinemachine.Instance.SetBossMutationCamera();
        });
        
    }

    
    public void BossMutationSequence()
    {
        ManagerSave.Instance.SaveState.isFirstBossDefeated = true;
        ManagerSave.Instance.Save();

        playerAnimator.SetTrigger("PlayerBossMutation");
        playerAnimator.applyRootMotion = true;
        ManagerCinemachine.Instance.SetBossMutationCamera();

        DOVirtual.DelayedCall(1f, () => 
        {

            DOTween.To(() => bossLegLeftMaterial.GetFloat("_DissolveAmount"),
                   x => bossLegLeftMaterial.SetFloat("_DissolveAmount", x),
                   0f,
                   1.7f).SetEase(Ease.OutSine);

            DOTween.To(() => bossLegRightMaterial.GetFloat("_DissolveAmount"),
                           x => bossLegRightMaterial.SetFloat("_DissolveAmount", x),
                           0f,
                           1.7f).SetEase(Ease.OutSine);
        });


    }



}
