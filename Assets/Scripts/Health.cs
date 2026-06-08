using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public float health;
    public float maxHealth = 100;

    [SerializeField] private Healthbar healthbar;

    public event Action OnDeath;

    private void Awake()
    {
        health = maxHealth;
        UpdateHealthBar(maxHealth, health);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            Die();
        }
        UpdateHealthBar(maxHealth, health);
    }

    private void UpdateHealthBar(float maxHealth, float health)
    {
        if (healthbar != null)
        {
            healthbar.UpdateHealthBar(maxHealth, health);
        }
        else return;
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
