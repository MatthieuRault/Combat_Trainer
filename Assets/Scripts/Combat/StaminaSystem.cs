using UnityEngine;
using UnityEngine.Events;

public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float regenRate = 12f;     // par seconde
    public float regenDelay = 1.5f;   // délai avant régén après une action

    private float _currentStamina;
    private float _regenTimer;

    public UnityEvent<float> onStaminaChanged;

    public float Current => _currentStamina;
    public float Ratio => _currentStamina / maxStamina;

    void Awake()
    {
        _currentStamina = maxStamina;
    }

    void Update()
    {
        // On attend le délai avant de régénérer
        if (_regenTimer > 0)
        {
            _regenTimer -= Time.deltaTime;
            return;
        }

        // Régénération progressive
        if (_currentStamina < maxStamina)
        {
            _currentStamina = Mathf.Min(maxStamina, _currentStamina + regenRate * Time.deltaTime);
            onStaminaChanged?.Invoke(_currentStamina);
        }
    }

    // Retourne false si pas assez de stamina
    public bool Use(float amount)
    {
        if (_currentStamina < amount) return false;

        _currentStamina -= amount;
        _regenTimer = regenDelay;
        onStaminaChanged?.Invoke(_currentStamina);
        return true;
    }
}