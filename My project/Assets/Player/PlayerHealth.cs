using UnityEngine;

/// <summary>
/// Simple PlayerHealth component for testing the enemy attack system.
/// This can be replaced with your actual player health system.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Configuration")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Events")]
    public System.Action<float> OnHealthChanged;
    public System.Action OnPlayerDied;
    
    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Take damage and update health
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Heal the player
    /// </summary>
    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"Player healed for {healAmount}. Health: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// Set health to maximum
    /// </summary>
    public void FullHeal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        
        Debug.Log("Player fully healed!");
    }
    
    private void Die()
    {
        OnPlayerDied?.Invoke();
        Debug.Log("Player died!");
        
        // Add death logic here (disable controls, play death animation, etc.)
        // For now, just disable the player
        gameObject.SetActive(false);
    }
    
    // Getters
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;
    public bool IsAlive() => currentHealth > 0;
}
