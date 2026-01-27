using DG.Tweening;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Player player;
    [SerializeField]private Collider leftAttackCollider;
    [SerializeField]private Collider rightAttackCollider;

    [Header("Combo Settings")]
    public int comboStep = 0; // 0, 1, or 2
    public float lastClickTime;
    public float comboResetTime = 0.8f;
    [SerializeField] private Transform bitePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player= GetComponent<Player>();
        if (leftAttackCollider != null) leftAttackCollider.enabled = false;
        if (rightAttackCollider != null) rightAttackCollider.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastClickTime > comboResetTime)
        {
            comboStep = 0;
        }
    }
    public void Attack()
    {
        lastClickTime = Time.time;

        player.playerAnimations.PlayComboAnimation(comboStep);

        // Advance to next hit (0 -> 1 -> 2 -> reset to 0)
        comboStep++;
        if (comboStep > 2)
        {
            comboStep = 0;
        }
    }
    public void SpawnBiteParticle() 
    {
        ManagerObjectPool.Instance.Spawn(ObjectPoolType.BiteParticle,bitePos);
        
        
    } 
    public void EnableLeftAttackCollider() => leftAttackCollider.enabled = true;
    public void DisableLeftAttackCollider() => leftAttackCollider.enabled = false;

    public void EnableRightAttackCollider() => rightAttackCollider.enabled = true;
    public void DisableRightAttackCollider() => rightAttackCollider.enabled = false;

}
