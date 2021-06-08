using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Button used to change the purchase amount
/// </summary>
public class BulkButton : MonoBehaviour {

    //Current position of the bulk purchasing
    private int m_bulkAmountPosition;

    //Bulk purchase amounts
    private int[] m_bulkAmounts = new int[] { 1, 5, 10, 50 };

    //The text to display on the button
    private TextMeshProUGUI m_displayText;


    /// <summary>
    /// Initialization of the bulk button
    /// </summary>
    void Start ()
    {
        m_bulkAmountPosition = 0;
        m_displayText = GameObject.Find("BulkBuyText").GetComponent<TextMeshProUGUI>();
    }
	
    /// <summary>
    /// Update the bulk buy amount for the button
    /// </summary>
    public void UpdateBulkAmount()
    {
        //Increment the position in the array of the buy amount to update the proper values
        m_bulkAmountPosition += 1;
        if (m_bulkAmountPosition >= m_bulkAmounts.Length)
        {
            m_bulkAmountPosition = 0;
        }
        //Update the backend and the visuals with the new bulk buy information
        ProcessGameLogic.SetBulkPurchaseAmount(m_bulkAmounts[m_bulkAmountPosition]);
        m_displayText.text = "x" + m_bulkAmounts[m_bulkAmountPosition];
    }
}
