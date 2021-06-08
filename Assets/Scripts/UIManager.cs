using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for managing certain UI elements
/// </summary>
public class UIManager : MonoBehaviour {

    // The blue team players TODO: m_
    [SerializeField]
    private Image[] bluePlayers;

    //The red team players TODO: m_
    [SerializeField]
    private Image[] redPlayers;

    // The blue team goalie TODO: m_
    [SerializeField]
    private Image blueGoalie;

    //The red team goalie TODO: m_
    [SerializeField]
    private Image redGoalie;

    //Blue goalie sprint TODO: m_
    [SerializeField]
    private Sprite blueGoalieSprite;

    //Red goalie sprite TODO: m_
    [SerializeField]
    private Sprite redGoalieSprite;

    //Empty image to hide elements. TODO: m_
    [SerializeField]
    private Sprite blankImage;

    //Blue players without sticks TODO: m_
    [SerializeField]
    private Sprite blueNoStick;

    //Red players with no sticks TODO: m_
    [SerializeField]
    private Sprite redNoStick;

    //Blue player stick TODO: m_
    [SerializeField]
    private Sprite blueStick;

    //Red player stick TODO: m_
    [SerializeField]
    private Sprite redStick;

    //Blue player tape TODO: m_
    [SerializeField]
    private Sprite blueTape;

    //Red player tape TODO: m_
    [SerializeField]
    private Sprite redTape;

    /// <summary>
    /// On purchase of a generator. See if we need to update the UI
    /// </summary>
    /// <param name="allGenerators">All current generators</param>
    public void CheckForUpdatedImages(Generator[] allGenerators)
    {
        int numTape = 0;
        int numStick = 0;
        int numOpponents = 0;
        int numGoalie = 0;
        Sprite blueImg;
        Sprite redImg;
        foreach (Generator gen in allGenerators)
        {
            if (gen.name == "Goalie" && gen.GetNumResources() > 0)
            {
                blueGoalie.sprite = blueGoalieSprite;
                numGoalie = gen.GetNumResources();
            }
            else if (gen.name == "Goalie")
            {
                blueGoalie.sprite = blankImage;
                redGoalie.sprite = blankImage;
            }
            else if (gen.name == "Sticks")
            {
                numStick = gen.GetNumResources();
            }
            else if (gen.name == "Tape")
            {
                numTape = gen.GetNumResources();
            }
            else if (gen.name == "Opponents")
            {
                numOpponents = gen.GetNumResources();
            }
        }
        if (numGoalie > 0 && numOpponents > 0)
        {
             redGoalie.sprite = redGoalieSprite;
        }
        if (numTape > 0)
        {
            blueImg = blueTape;
            
            redImg = redTape;
        }
        else if (numStick > 0)
        {
            blueImg = blueStick;
            redImg = redStick;
        }
        else
        {
            blueImg = blueNoStick;
            redImg = redNoStick;
        }

        foreach (Image img in bluePlayers)
        {
            img.sprite = blueImg;
        }
        if (numOpponents == 0)
        {
            redImg = blankImage;
        }
        foreach (Image img in redPlayers)
        {
            img.sprite = redImg;
        }
    }

    /// <summary>
    /// Show a UI object
    /// </summary>
    /// <param name="name">The name of the object</param>
    public static void ShowUIObject(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                GameObject elementToShow = obj;
                elementToShow.SetActive(true);
                break;
            }
        }

    }

    /// <summary>
    /// Hide UI object
    /// </summary>
    /// <param name="name">The object  ot hide</param>
    public static void HideUIObject(string name)
    {
        GameObject elementToShow = GameObject.Find(name);
        if (elementToShow != null)
        {
            elementToShow.SetActive(false);
        }
        else
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == name)
                {
                    elementToShow = obj;
                    break;
                }
            }
            elementToShow.SetActive(false);
        }

    }

    //Name of the offline pane
    public static string OFFLINE_PANEL_UI = "OfflineTimeUI";
    //Name of the main game UI
    public static string MAIN_GAME_PANEL_UI = "MainGameUI";
    //Name of the statistics UI
    public static string STATISTICS_PANEL_UI = "StatisticsUI";
    //Name ofthe shop UI
    public static string SHOP_PANEL_UI = "ShopUI";
    //Name of the alert UI
    public static string ALERT_IMAGE = "Alert";
    //Name of the information pane
    public static string INFORMATION_UI = "InformationUI";
    //Name of the minigame
    public static string MINIGAME_UI = "MiniGameUI";
}
