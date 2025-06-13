using ERA;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        EventManager.Trigger($"{other.gameObject.GetInstanceID()}.ApplyDamage", damage);
    }
}
