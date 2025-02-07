using UnityEngine;

namespace Example.Event_Driven_Architecture_Example.Scripts.ExampleTriggerEvent
{
    public class EnemyAttack: Subscriber
    {
        [SerializeField] private int damage;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag("Player")) return;
            
            EventManager.Publish($"{other.gameObject.GetInstanceID()}.ApplyDamage",damage);
        }
    }
}