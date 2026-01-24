using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController player; 
    [SerializeField] private Collider leftAttackCollider;
    [SerializeField] private Collider rightAttackCollider;

    [Header("Combo Settings")]
    public int comboStep = 0;
    public float lastClickTime;
    public float comboResetTime = 0.8f;
    [SerializeField] private Transform bitePos;

    void Start()
    {
        player = GetComponent<PlayerController>(); // Değişti
        if (leftAttackCollider != null) leftAttackCollider.enabled = false;
        if (rightAttackCollider != null) rightAttackCollider.enabled = false;
    }

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
        

        player.AnimationEvents.PlayComboAnimation(comboStep);

        comboStep++;
        if (comboStep > 2) comboStep = 0;
    }

    public void SpawnBiteParticle() 
    {
        ManagerObjectPool.Instance.Spawn(ObjectPoolType.BiteParticle, bitePos);
    } 

    public void EnableLeftAttackCollider() => leftAttackCollider.enabled = true;
    public void DisableLeftAttackCollider() => leftAttackCollider.enabled = false;
    public void EnableRightAttackCollider() => rightAttackCollider.enabled = true;
    public void DisableRightAttackCollider() => rightAttackCollider.enabled = false;
}