using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerController player;
    private bool cameraFiredThisSwing; 

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        cameraFiredThisSwing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        enemy.TakeDamage(35);
        
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        // B. Efektin Yönünü Hesapla (Düşmanın içinden dışarı doğru)
        // Mantık: (Vurulan Nokta) - (Düşmanın Merkezi) = Dışarı Bakan Vektör
        Vector3 impactNormal = (hitPoint - other.transform.position).normalized;
        
        // C. Bu vektöre göre rotasyon oluştur
        Quaternion hitRotation = Quaternion.LookRotation(impactNormal);

        ManagerObjectPool.Instance.Spawn(ObjectPoolType.ClawParticle, hitPoint, hitRotation);
        // 4. Efekti Çağır (Hesapladığımız rotasyon ile)
        
        
        
        // FİNAL VURUŞ KONTROLÜ
        if (player != null && player.IsFinalComboActive && !cameraFiredThisSwing)
        {
            cameraFiredThisSwing = true; // Bu vuruş için kilit vur

            // Kamerayı ve yavaşlatmayı başlat
            if(ManagerCinemachine.Instance != null)
                ManagerCinemachine.Instance.TriggerFinisherCamera();
        }
    }
}