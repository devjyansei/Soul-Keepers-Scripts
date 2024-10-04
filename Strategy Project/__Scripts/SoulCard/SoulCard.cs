using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using System;

public class SoulCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Image cardImage;
    [SerializeField] Button button;
    [SerializeField] Gradient gradient;
    [SerializeField] ParticleSystem boughtParticle;

    [SerializeField] Vector3 endScale;
    [SerializeField] Vector3 startScale;

    [SerializeField] Vector3 cardPos;

    public Card card;
    [SerializeField] Image[] cardImages;

    

    Vector3 buyPos;
    float duration;
    private float moveCooldown = 0.1f;
    private bool isMoved = false;
    private bool isMoving = false;
    [SerializeField] private bool isBought = false;
    public bool canBuyable = true;

    private int currentIndex = 0;

    private void Start()
    {
        
        SetCard();
        

    }
    private void OnEnable()
    {
        transform.position = cardPos;
    }
    private void Update()
    {
        duration += Time.unscaledTime;
        
    }

    // CARD SCALING
    public void OnPointerEnter(PointerEventData eventData)
    {

        if (!isBought && GameManager.Instance.GetIsPaused())
        {
            cardPos = transform.position;
            AudioManager.Instance.PlayCardOnPointerEnterSound();


            if (!isMoved && !isMoving && duration > moveCooldown)
            {
                
                isMoving = true;
                float moveTime = .1f;
                Tween moveTween = transform.DOScale(endScale, moveTime).OnComplete(() => { isMoved = true; }).OnComplete(() => { isMoving = false; });
                moveTween.SetUpdate(true);
                duration = 0;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isBought)
        {
            

            isMoving = true;
            float moveTime = .1f;
            Tween moveTween = transform.DOScale(startScale, moveTime).OnComplete(() => { isMoved = false; }).OnComplete(() => { isMoving = false; });
            moveTween.SetUpdate(true);
        }

    }

    public void BuySoulCard()
    {
        AudioManager.Instance.PlayCardBuySound();
        button.interactable = false;
        isBought = true;
        ResourceManager.Instance.DecreaseCurrentSoulAmount(card.cost);

        EventManager.OnResourcesUpdated();
        StartCoroutine( MeltEffect());
    }

    private IEnumerator MeltEffect()
    {        
        while (currentIndex < cardImages.Length-1)
        {
            cardImages[currentIndex + 1].gameObject.SetActive(true);
            currentIndex++;
            cardImages[currentIndex - 1].gameObject.SetActive(false);
            
            
            yield return new WaitForSecondsRealtime(.01f);
        }

    }

    

    public void SetCard()
    {
        nameText.text = card.name;
        descriptionText.text = card.description;
        costText.text = card.cost.ToString();
    }
}
