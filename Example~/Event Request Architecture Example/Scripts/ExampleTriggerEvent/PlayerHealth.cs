using ERA;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int _currentHealth;

    private void Awake()
    {
        EventManager.AddEvent<int>($"{gameObject.GetInstanceID()}.ApplyDamage", gameObject, ApplyDamage);
    }

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    private void ApplyDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);

        print(_currentHealth);
    }
}
