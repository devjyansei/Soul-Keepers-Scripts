using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickEventsManager : MonoSingleton<ClickEventsManager>
{
    public Button.ButtonClickedEvent defaultAttribute;//null

    public Button.ButtonClickedEvent groupSelectButton;

    //UNITS
    public List<Button.ButtonClickedEvent> workerAttributesList = new List<Button.ButtonClickedEvent>();    
    public List<Button.ButtonClickedEvent> paladinAttributesList = new List<Button.ButtonClickedEvent>();
    public List<Button.ButtonClickedEvent> archerAttributesList = new List<Button.ButtonClickedEvent>();

    

    //BUILDINGS
    public List<Button.ButtonClickedEvent> coreAttributesList = new List<Button.ButtonClickedEvent>();
    public List<Button.ButtonClickedEvent> houseAttributesList = new List<Button.ButtonClickedEvent>();
    public List<Button.ButtonClickedEvent> woodGeneratorAttributesList = new List<Button.ButtonClickedEvent>();
    public List<Button.ButtonClickedEvent> stoneGeneratorAttributesList = new List<Button.ButtonClickedEvent>();
    public List<Button.ButtonClickedEvent> archerTowerAttributesList = new List<Button.ButtonClickedEvent>();
    public List<Button.ButtonClickedEvent> academyAttributesList = new List<Button.ButtonClickedEvent>();

    

}
