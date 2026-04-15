using TMPro;
using UnityEngine;

public class WormHealth : MonoBehaviour
{
    private int _health;
    public int MaxHealth = 100;

    [SerializeField] private TMP_Text _healthText;

    private Worm _worm; // reference to Worm script

    void Start()
    {
        _health = MaxHealth;
        _healthText.SetText(_health.ToString());

        _worm = GetComponent<Worm>(); // get Worm script
    }

    internal void ChangeHealth(int change)
    {
        _health += change;

        if (_health > MaxHealth)
            _health = MaxHealth;
        else if (_health <= 0)
        {
            _health = 0;
            Die();
        }

        _healthText.SetText(_health.ToString());
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died!");

        _worm.IsAlive = false;

        gameObject.SetActive(false);

        WormManager.Instance.NextWorm();
    }
}