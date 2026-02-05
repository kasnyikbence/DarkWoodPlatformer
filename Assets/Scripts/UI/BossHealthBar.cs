using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI bossNameText;

    private Damageable bossDamageable;

    public void Initialize(BossController boss)
    {
        bossDamageable = boss.GetComponent<Damageable>();

        if (bossDamageable != null)
        {
            bossDamageable.healthChanged.AddListener(UpdateHealth);

            healthSlider.maxValue = bossDamageable.MaxHealth;
            healthSlider.value = bossDamageable.Health;

            gameObject.SetActive(true);
        }
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (bossDamageable != null)
        {
            bossDamageable.healthChanged.RemoveListener(UpdateHealth);
        }
    }
}