using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoHealthBar : MonoBehaviour
{
    [SerializeField] private PlayerCargoState cargoState;
    [SerializeField] public Image foreSprite;
    [SerializeField] private GameObject root;
    [SerializeField] public TMP_Text healthNum;
    [SerializeField] private int maxHealth = 5;

    private void Awake()
    {
        if (cargoState == null)
        {
            cargoState = GetComponentInParent<PlayerCargoState>();
        }
    }

    private void OnEnable()
    {
        cargoState.CargoHealth.OnValueChanged += OnHealthChanged;
        updateHealthBar(cargoState.CargoHealth.Value);
    }

    private void OnDisable()
    {
        cargoState.CargoHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        updateHealthBar(newValue);
    }

    public void updateHealthBar(float currentHealth)
    {
        foreSprite.fillAmount = currentHealth / maxHealth;
        healthNum.text = $"{currentHealth}/{maxHealth}";
        root.SetActive(currentHealth > 0);
    }
}
