using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TooltipManager : MonoBehaviour 
{
    public static TooltipManager _instance;
    [SerializeField] private Image unitTooltip;

    public TextMeshProUGUI tooltipText;

    float screenWidth = Screen.width;
    float screenHeight = Screen.height;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
        
    }
    
    private void OnDisable()
    {
        //ekranýn görunmeyen bir kýsmýnda pozisyonunu sýfýrlar
        transform.position = new Vector3(screenWidth*2, screenHeight*2, 0);
    }
    private void Update()
    {
        if (UnitSelections.Instance.selectedUnit == null && BuildingManager.Instance.selectedStructurePrefab == null)
        {
            gameObject.SetActive(false);
        }

        transform.position = Input.mousePosition + new Vector3(unitTooltip.rectTransform.pivot.x, unitTooltip.rectTransform.pivot.y,0);
    }
    public void SetAndShowTooltip(string message)
    {
        tooltipText.text = message;
        gameObject.SetActive(true);
    }
    public void HideTooltip()
    {
        tooltipText.text = string.Empty;
        gameObject.SetActive(false);
        


    }
    public void SetTooltipScale()
    {

        int numberOfLines = tooltipText.textInfo.lineCount;
        float messageLength = tooltipText.preferredWidth;
        Vector2 imageScale =unitTooltip.rectTransform.sizeDelta;

        imageScale.x = messageLength + tooltipText.fontSize*2;
        imageScale.y = numberOfLines * tooltipText.fontSize ;

        unitTooltip.rectTransform.sizeDelta = imageScale;
        //Debug.Log(tooltipText.textInfo.lineCount);

    }
    
}