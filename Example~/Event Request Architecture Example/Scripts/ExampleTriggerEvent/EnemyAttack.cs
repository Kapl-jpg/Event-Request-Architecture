using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleTriggerEvent
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private int damage;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            EventManager.Trigger($"{other.gameObject.GetInstanceID()}.ApplyDamage", damage);
        }
    }
}