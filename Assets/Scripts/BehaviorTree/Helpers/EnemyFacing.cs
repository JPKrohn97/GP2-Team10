using UnityEngine;

namespace BehaviorTree
{
    public static class EnemyFacing
    {
        // Instant direction change - left/right only (Z axis movement)
        // Use 0° for right (+Z) and 180° for left (-Z)
        public static void FaceTarget(Transform enemy, Transform target)
        {
            if (target == null) return;
            
            float direction = target.position.z - enemy.position.z;
            
            // Instant rotation: 0° (right/+Z) or 180° (left/-Z)
            if (direction > 0)
                enemy.rotation = Quaternion.Euler(0, 0, 0);   // Right (+Z)
            else
                enemy.rotation = Quaternion.Euler(0, 180, 0); // Left (-Z)
        }
        
        public static void FaceDirection(Transform enemy, Vector3 targetPosition)
        {
            float direction = targetPosition.z - enemy.position.z;
            
            if (direction > 0)
                enemy.rotation = Quaternion.Euler(0, 0, 0);   // Right (+Z)
            else
                enemy.rotation = Quaternion.Euler(0, 180, 0); // Left (-Z)
        }
        
        // Returns position constrained to Z-axis movement only
        public static Vector3 GetConstrainedPosition(Vector3 enemyPos, Vector3 targetPos)
        {
            return new Vector3(enemyPos.x, enemyPos.y, targetPos.z);
        }
    }
}