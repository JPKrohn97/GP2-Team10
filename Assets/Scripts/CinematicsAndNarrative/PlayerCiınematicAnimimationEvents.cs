using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;  
public class PlayerCiınematicAnimimationEvents : MonoBehaviour
{
    public bool isSpecialJump;
    public GameObject fakeBoss;
    private Transform playerTransform;
    private PlayerController playerController;
    public Animator animController;
    private void Awake() 
    {
        playerTransform=GetComponentInParent<PlayerController>().transform;
        playerController=GetComponentInParent<PlayerController>();
        animController=GetComponent<Animator>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown("V"))
        //{
        //    SpecialJumpButton();
        //}
    }
    public void SpecialJumpButton()
    {       
        animController.SetTrigger("SpecialJump");
        GameManager.Instance.canPlayerMove = false;
        Vector3 v = playerController.RB.linearVelocity;
        v.y = Mathf.Sqrt(playerController.jumpHeight * -2f * Physics.gravity.y);
        playerController.RB.linearVelocity = v;
    }

    public void SpecialJump()
    {
        isSpecialJump = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void FadeIn()
    {
        GameManager.Instance.CinematicBlackFadeIn(0.5f);
        
    }
    public void FadeOut()
    {
        GameManager.Instance.CinematicBlackFadeOut(0.5f);
    }
    public void Beginning()
    {
        fakeBoss.SetActive(true);
        transform.parent.eulerAngles = new Vector3(0, 180, 0);
        ManagerCinemachine.Instance.FirstBiteCamera();


    }
    public void BeforeFirstBite()
    {

        FadeOut();

    }
    public void FirstBite()
    {
        FadeIn();
    }

    public void BeforeSecondBite()
    {
        ManagerCinemachine.Instance.SecondBiteCamera();
        transform.parent.eulerAngles = new Vector3(0, 0, 0); 
        fakeBoss.transform.localPosition = new Vector3(-0.5f, 0, 0);
        FadeOut();
    }
    public void SecondBite()
    {
        FadeIn();
    }
 
    public void NormalGame()
    {
        animController.applyRootMotion = false;
        GameManager.Instance.NormalGamePlay();
    }
    public void End()
    {
        transform.parent.eulerAngles = new Vector3(0, 180, 0);
        fakeBoss.SetActive(false);
        FadeOut();
        DOVirtual.DelayedCall(0.5f, () => GameManager.Instance.BossMutationSequence());
    }
}
