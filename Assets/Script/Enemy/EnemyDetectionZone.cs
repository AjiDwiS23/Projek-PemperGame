using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyDetectionZone : MonoBehaviour
{
    private EnemyController_2D parentEnemy;
    private CircleCollider2D col;


    public void Init(EnemyController_2D enemy, float radius)
    {
        parentEnemy = enemy;

        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = radius;
    }

    public void SetRadius(float radius)
    {
        if (col != null) col.radius = radius;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (parentEnemy == null) return;

        if (((1 << other.gameObject.layer) & parentEnemy.playerLayer) != 0)
            parentEnemy.OnPlayerEnter(other.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (parentEnemy == null) return;

        if (((1 << other.gameObject.layer) & parentEnemy.playerLayer) != 0)
            parentEnemy.OnPlayerExit();
    }
}