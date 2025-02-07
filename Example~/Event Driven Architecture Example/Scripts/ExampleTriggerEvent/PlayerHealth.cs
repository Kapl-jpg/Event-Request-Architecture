using UnityEngine;

namespace Example.Event_Driven_Architecture_Example.Scripts.ExampleTriggerEvent
{
    public class PlayerHealth : Subscriber
    {
        [SerializeField] private int maxHealth;

        private int _currentHealth;

        private void Start()
        {
            _currentHealth = maxHealth;
        }

        [Event(true, "ApplyDamage")]
        private void ApplyDamage(object data)
        {
            if (data is not int damage) return;
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
            
            print(_currentHealth);
        }
    }
}
