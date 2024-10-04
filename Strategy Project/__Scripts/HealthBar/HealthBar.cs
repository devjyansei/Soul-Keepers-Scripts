using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//in healthbar image
public class HealthBar : MonoBehaviour
{
    private Image _healthBarImage;
    [SerializeField] private float _timeToDrain = 0.2f;
    [SerializeField] private Gradient _healthBarGradient;
    private Color _newHealthBarColor;
    private float _targetPercentage = 1f;
    private Coroutine drainHealthBarCoroutine;
    private void Awake()
    {
        _healthBarImage = GetComponent<Image>();
    }
    private void Start()
    {
        //set the health bar color
        _healthBarImage.color = _healthBarGradient.Evaluate(_targetPercentage);

        CheckHealthBarGradientAmount();
    }
    public void UpdateHealthbar(float maxHealth,float currentHealth)
    {
        if (!GameManager.Instance.IsGameEnded())
        {
            _targetPercentage = currentHealth / maxHealth;


            drainHealthBarCoroutine = StartCoroutine(DrainHealthBar());
            CheckHealthBarGradientAmount();
        }
        
    }
    private IEnumerator DrainHealthBar()
    {
        if (!GameManager.Instance.IsGameEnded())
        {
            float fillAmount = _healthBarImage.fillAmount;
            Color currentColor = _healthBarImage.color;
            float elapsedTime = 0f;

            while (elapsedTime < _timeToDrain)
            {

                elapsedTime += Time.deltaTime;
                //lerp the fill amount
                _healthBarImage.fillAmount = Mathf.Lerp(fillAmount, _targetPercentage, (elapsedTime / _timeToDrain));

                //lerp the color based on the gradient
                _healthBarImage.color = Color.Lerp(currentColor, _newHealthBarColor, (elapsedTime / _timeToDrain));
                yield return null;
            }
        }
        
        
    }
    private void CheckHealthBarGradientAmount()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_targetPercentage);

    }
}
