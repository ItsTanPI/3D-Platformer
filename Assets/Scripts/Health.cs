using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] int _maxHealth = 100;
    public int _currentHealth;


    public bool isPlayer = false;
    [SerializeField] Image _image;

    [SerializeField] GameObject _gameObject;
    private void Awake()
    {
        _currentHealth = _maxHealth;
    }


    private void Update()
    {
        
        if (_image != null) 
        {
            _image.fillAmount = Mathf.Lerp(_image.fillAmount, (float)_currentHealth / (float)_maxHealth, Time.deltaTime * 3);
        }
    }
    public void TakeDamage(int amount = - 1)
    {
        if (isPlayer)
        {
            GetComponent<CinemachineImpulseSource>().GenerateImpulse(0.1f);
            FindObjectOfType<DamageVignette>().FlashOnDamage();
        }
        if (amount <= -1 ) amount = _currentHealth;
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;

        if (_currentHealth >= _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    private void Die()
    {
        if ( _gameObject != null ) Instantiate(_gameObject, transform.position, transform.rotation); 
        Destroy(gameObject);
    }
}
