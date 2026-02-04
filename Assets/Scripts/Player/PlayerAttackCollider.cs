using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerController player;
    private bool cameraFiredThisSwing;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        cameraFiredThisSwing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        SoundManager.Instance.PlaySound(SoundManager.Instance.ClawsImpact);
        //SoundManager.Instance.PlaySoundOneShot(SoundManager.Instance.ClawsImpact,transform.position);

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;
        col.enabled = false;    
        enemy.TakeDamage(35);
        
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        Vector3 impactNormal = (hitPoint - other.transform.position).normalized;
        
    
        Quaternion hitRotation = Quaternion.LookRotation(impactNormal);

        ManagerObjectPool.Instance.Spawn(ObjectPoolType.ClawParticle, hitPoint, hitRotation);

        

        if (player != null && player.IsFinalComboActive && !cameraFiredThisSwing)
        {
            cameraFiredThisSwing = true; 
            
            if(ManagerCinemachine.Instance != null)
                ManagerCinemachine.Instance.TriggerFinisherCamera();
        }
    }
}