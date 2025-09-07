using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleTriggerEvent
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth;

        private int _currentHealth;

        private void Start()
        {
            EventManager.Subscribe<int>($"{gameObject.GetInstanceID()}.ApplyDamage",gameObject, ApplyDamage);
            _currentHealth = maxHealth;
        }

        private void ApplyDamage(int damage)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
            
            print(_currentHealth);
        }
    }
}
