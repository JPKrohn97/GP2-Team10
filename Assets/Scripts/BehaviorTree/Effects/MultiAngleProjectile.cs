using UnityEngine;
using System.Collections;

public class MultiAngleProjectile : MonoBehaviour
{
    private Transform firePoint;
    private Transform target;
    private int damage;
    private float projectileSpeed;
    private float[] angles = { 30, 20, 10, 0f }; // Last shot is direct (0 degrees from horizontal)
    private float delayBetweenShots = 0.25f;

    public void Initialize(Transform firePoint, Transform target, int damage, float speed)
    {
        this.firePoint = firePoint;
        this.target = target;
        this.damage = damage;
        this.projectileSpeed = speed;
        
        StartCoroutine(ShootSequence());
    }

    private IEnumerator ShootSequence()
    {
        Vector3 baseDirection = (target.position - firePoint.position).normalized;
        
        // Shoot 4 projectiles with increasing angles (from ground upward to direct)
        for (int i = 0; i < angles.Length; i++)
        {
            ShootProjectileAtAngle(baseDirection, angles[i]);
            yield return new WaitForSeconds(delayBetweenShots);
        }
        
        Destroy(gameObject);
    }
    
    private void ShootProjectileAtAngle(Vector3 baseDirection, float angleOffset)
    {
        // Get horizontal direction (XZ plane)
        Vector3 horizontalDir = new Vector3(baseDirection.x, 0, baseDirection.z).normalized;
        
        // Calculate shoot direction with vertical angle offset
        // Negative angle points downward, 0 is horizontal
        Vector3 shootDirection = Quaternion.AngleAxis(-angleOffset, Vector3.Cross(horizontalDir, Vector3.up)) * horizontalDir;
        
        GameObject projectile = ManagerObjectPool.Instance.Spawn(
            ObjectPoolType.BossProjectile,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        if (projectile != null)
        {
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = shootDirection * projectileSpeed;
            }

            var projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.damage = damage;
            }
        }
    }
}
