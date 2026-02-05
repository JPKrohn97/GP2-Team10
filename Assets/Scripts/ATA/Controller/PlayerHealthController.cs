using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    private Tween healthTween;
    [SerializeField] private float healthAnimDuration = 0.25f;
    [SerializeField] private Ease healthEase = Ease.OutCubic;

    [Header("UI References")]
    public Slider healthSlider; 
    public Image damageImage; 
    
    [Header("References")]
    public SkinnedMeshRenderer playerMesh; 
    public Color flashColor = Color.red; 
    public float flashDuration = 0.3f;
    private Material playerMat;
    private Color originalColor;
    private Tween flashTween;

    private PlayerController playerController;
    private Animator animator;
    private bool isDead = false;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        
        if (playerMesh != null)
        {

            playerMat = playerMesh.material;

            if (playerMat.HasProperty("_Color"))
                originalColor = playerMat.color;
            else
                originalColor = Color.white;
        }
        
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        
        DamageFlash();


        UpdateHealthUI();
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
           animator.SetTrigger("Hit");
        }
    }
    
    private void DamageFlash()
    {
        if (playerMat == null) return;

        flashTween?.Kill();

        playerMat.color = flashColor;

        flashTween = playerMat.DOColor(originalColor, flashDuration);
    }


    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
 
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;
        UpdateHealthUI();



        if (playerController != null)
        {
  
            playerController.InputHandler.Disable(); 
            playerController.enabled = false; 
            

            playerController.RB.linearVelocity = Vector3.zero;
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
            
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        GameManager.Instance.RestartScene();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = maxHealth;
        
        healthTween?.Kill();
        
        healthTween = DOTween.To(
            () => healthSlider.value,
            x => healthSlider.value = x,
            currentHealth,
            healthAnimDuration
        ).SetEase(healthEase);
    }
}