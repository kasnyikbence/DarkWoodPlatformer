using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damageable playerDamageable;


    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has tag 'Player'");
        }

        playerDamageable = player.GetComponent<Damageable>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthSlider.value = CalculateSliderPrecentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthBarText.text = "Health " + playerDamageable.Health + " / " + playerDamageable.MaxHealth;
    }

    void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private float CalculateSliderPrecentage(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    public void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CalculateSliderPrecentage(newHealth, maxHealth);
        healthBarText.text = "Health " + newHealth + " / " + maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
