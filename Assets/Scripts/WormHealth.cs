using TMPro;
using UnityEngine;

public class WormHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int MaxHealth = 100;

    [Header("UI Reference")]
    [SerializeField] private TMP_Text _healthText;

    // Current health
    private int _health;

    // Reference to Worm component
    private Worm _worm;

    void Start()
    {
        _health = MaxHealth;
        _worm = GetComponent<Worm>();

        UpdateHealthDisplay();

        Debug.Log($"{gameObject.name} initialized with {_health} HP");
    }

    /// <summary>
    /// Changes health by the given amount. Negative values deal damage.
    /// </summary>
    public void ChangeHealth(int change)
    {
        int previousHealth = _health;
        _health += change;

        // Clamp health
        if (_health > MaxHealth)
            _health = MaxHealth;
        else if (_health < 0)
            _health = 0;

        UpdateHealthDisplay();

        Debug.Log($"{gameObject.name}: Health {previousHealth} -> {_health}");

        // Check for death
        if (_health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Returns current health value.
    /// </summary>
    public int GetHealth()
    {
        return _health;
    }

    private void UpdateHealthDisplay()
    {
        if (_healthText != null)
        {
            _healthText.SetText(_health.ToString());
        }
    }

    private void Die()
    {
        Debug.Log($"");
        Debug.Log($"*** {gameObject.name} DIED! ***");
        Debug.Log($"");

        // Mark worm as dead
        if (_worm != null)
        {
            _worm.IsAlive = false;
        }

        // Notify WormManager that this worm died
        // This will check for a winner
        if (WormManager.Instance != null)
        {
            WormManager.Instance.OnWormDied(_worm);
        }

        // Disable the worm GameObject
        // Using SetActive(false) to completely disable it
        gameObject.SetActive(false);
    }
}
