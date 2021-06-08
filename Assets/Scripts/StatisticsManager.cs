using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class for managing the game statistics
/// </summary>
public class StatisticsManager : MonoBehaviour {

    //Lifetime income
    private static NumberWithModifier m_lifeTimeIncome;

    //Text to show the lifetime income
    private TextMeshProUGUI m_lifeTimeIncomeText;

    //Text to show the current skill level
    private TextMeshProUGUI m_prestigeLevelText;

    //Text to show the username
    private TextMeshProUGUI m_usernameText;


    void Start()
    {
        m_lifeTimeIncomeText = GameObject.Find("LifetimeTotalIncome").GetComponent<TextMeshProUGUI>();
        m_prestigeLevelText = GameObject.Find("Prestige Level").GetComponent<TextMeshProUGUI>();
        m_usernameText = GameObject.Find("Username").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        m_prestigeLevelText.text = DataManager.GetCurrentPrestigeName() + " (" + (float)DataManager.GetCurrentPrestigeMultiplier() + "x)";
        m_lifeTimeIncomeText.text = NumberWithModifier.ToString(m_lifeTimeIncome);
        m_usernameText.text = DataManager.GetUsername();

    }

    /// <summary>
    /// Add to the total lifetime income
    /// </summary>
    /// <param name="num">The amount to add</param>
    public static void AddToLifetimeIncome(NumberWithModifier num)
    {
        m_lifeTimeIncome += num;
    }

    /// <summary>
    /// Gets the total lifetime income
    /// </summary>
    /// <returns>The total lifetime income</returns>
    public static NumberWithModifier GetLifetimeIncome()
    {
        return m_lifeTimeIncome;
    }

    /// <summary>
    /// Sets the total lifetime income
    /// </summary>
    /// <param name="num">The total lifetime income</param>
    public static void SetLifetimeIncome(NumberWithModifier num)
    {
        m_lifeTimeIncome = num;
    }

    /// <summary>
    /// Sets the username of the player
    /// </summary>
    public void SetUsername()
    {
        DataManager.SetUsername(m_usernameText.text);
    }




}
