using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Barres joueur")]
    public Slider healthBar;
    public Slider staminaBar;

    [Header("Textes")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;

    [Header("Références joueur")]
    public HealthSystem playerHealth;
    public StaminaSystem playerStamina;

    void Start()
    {
        playerHealth.onHealthChanged.AddListener(_ => UpdateHealth());
        playerStamina.onStaminaChanged.AddListener(_ => UpdateStamina());

        UpdateHealth();
        UpdateStamina();
    }

    void UpdateHealth()
    {
        healthBar.value = playerHealth.Ratio;
        healthText.text = $"{(int)playerHealth.Current}/{(int)playerHealth.maxHealth}";
    }

    void UpdateStamina()
    {
        staminaBar.value = playerStamina.Ratio;
        staminaText.text = $"{(int)playerStamina.Current}/{(int)playerStamina.maxStamina}";
    }
}