using UnityEngine;

namespace BehaviorTree
{
    public static class EnemyFacing
    {
        // Instant direction change - left/right only (Z axis movement)
        public static void FaceTarget(Transform enemy, Transform target)
        {
            if (target == null || enemy == null) return;

            float direction = target.position.z - enemy.position.z;

            // Only change facing if there's meaningful difference
            if (Mathf.Abs(direction) > 0.01f)
            {
                if (direction > 0)
                    enemy.rotation = Quaternion.Euler(0, 0, 0);   // Right (+Z)
                else
                    enemy.rotation = Quaternion.Euler(0, 180, 0); // Left (-Z)
            }
        }

        public static void FaceDirection(Transform enemy, Vector3 targetPosition)
        {
            if (enemy == null) return;

            float direction = targetPosition.z - enemy.position.z;

            if (Mathf.Abs(direction) > 0.01f)
            {
                if (direction > 0)
                    enemy.rotation = Quaternion.Euler(0, 0, 0);   // Right (+Z)
                else
                    enemy.rotation = Quaternion.Euler(0, 180, 0); // Left (-Z)
            }
        }

        // Returns position constrained to Z-axis movement only
        public static Vector3 GetConstrainedPosition(Vector3 enemyPos, Vector3 targetPos)
        {
            return new Vector3(enemyPos.x, enemyPos.y, targetPos.z);
        }
    }
}
