using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public HealthSystem enemyHealth;
    public TextMeshProUGUI infoText;

    void Update()
    {
        if (enemyHealth != null && slider != null)
        {
            slider.value = enemyHealth.Ratio;

            if (infoText != null)
                infoText.text = $"{gameObject.transform.parent.name}\n{(int)enemyHealth.Current}/{(int)enemyHealth.maxHealth}";
        }

        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}