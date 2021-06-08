using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class MultiplierManager : MonoBehaviour {

    //Color used when the multiplier is active
    Color m_green;

    //Color used when the multiplier is inactive
    Color m_red;

    //The end time for the multiplier
    static DateTime m_endTime;

    //The hours we get for each multiplier ad watched
    private static int m_hours = 1;

    //Interactive button for starting the ad
    [SerializeField]
    private Button m_multiplierButton;

    //Text for the multiplier
    [SerializeField]
    private TextMeshProUGUI m_multiplerText;

	// Use this for initialization
	void Start ()
    {
        m_green = new Color(0.012f, 0.216f, 0.059f, 1.0f);
        m_red = new Color(0.216f, 0.008f, 0.0314f, 1.0f);
        m_endTime = DataManager.GetMultiplierEndTime();

    }

    // Update is called once per frame
    void Update ()
    {
        //If multiplier time expired
        if (DateTime.Compare(m_endTime, System.DateTime.Now) <= 0 )
        {
            ProcessGameLogic.SetAdMultiplier(1);
            m_multiplierButton.image.color = m_red;
            m_multiplerText.text = "No Multiplier";
            m_endTime = System.DateTime.Now;
        }
        //If multiplier is still active
        else
        {
            ProcessGameLogic.SetAdMultiplier(3);
            m_multiplierButton.GetComponent<Image>().color = m_green;
            TimeSpan remainingTime = m_endTime.Subtract(System.DateTime.Now);
            m_multiplerText.text = remainingTime.Hours.ToString("00") + ":"
            + remainingTime.Minutes.ToString("00") + ":"
            + remainingTime.Seconds.ToString("00") + " (3x)";
        }
	}

    /// <summary>
    /// Function called when an ad is successfully watched
    /// </summary>
    public static void AddHourOfMultiplier()
    {
        if ((m_endTime.AddHours(m_hours).Subtract(System.DateTime.Now).TotalSeconds > 21600))
        {
            m_endTime = System.DateTime.Now.AddHours(6);
        }
        else
        {
            m_endTime = m_endTime.AddHours(m_hours);
        }
    }

    /// <summary>
    /// Gets the end time of the multiplier
    /// </summary>
    /// <returns> A string for the end time of the multiplier </returns>
    public static string GetMultiplierEndTime()
    {
        return m_endTime.ToBinary().ToString();
    }

}
