using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Vie")]
    public float maxHealth = 100f;
    private float _currentHealth;

    // Événements appelés automatiquement quand on prend des dégâts ou qu'on meurt
    public UnityEvent<float> onHealthChanged; // pour mettre à jour la barre de vie
    public UnityEvent onDeath;

    public float Current => _currentHealth;
    public float Ratio => _currentHealth / maxHealth; // entre 0 et 1, pour la barre UI

    void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (_currentHealth <= 0) return; // déjà mort

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        onHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
            onDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
        onHealthChanged?.Invoke(_currentHealth);
    }
}