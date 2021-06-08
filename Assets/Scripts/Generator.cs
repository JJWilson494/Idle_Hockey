using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class representing an income generator. Contains all information
/// pertinent to the income, multiplier, and buying the resource.
/// </summary>
public class Generator : MonoBehaviour {

    //The number of resources we have of this generator
    [SerializeField]
    private int m_numResources;

    //The UI manager for updating information relating to the generator
    [SerializeField]
    private UIManager m_uiManager;

    /*
     * The base cost of the generator scaled to be less than 1000. Works in tandem with the 
     * base exponent to calculate the actual cost
    */
    [SerializeField]
    private float m_baseNumber;

    /*
     * The base exponent of the generator. Works in tandem with the 
     * base number to calculate the actual cost
    */
    [SerializeField]
    private int m_baseExponent;

    //The amount of income this generator produces
    [SerializeField]
    private float m_production;

    //The name of this generator
    [SerializeField]
    private TextMeshProUGUI m_resourceName;

    //The text representation of the number of resources 
    [SerializeField]
    private TextMeshProUGUI m_totalResources;

    //The button to buy additional resources
    [SerializeField]
    private Button m_buyButton;

    //The text representation of the price
    [SerializeField]
    private TextMeshProUGUI m_price;

    //The rate at which the generator increases income
    [SerializeField]
    private float m_growthRate;

    //The associated multipliers for this generator
    private int m_multipliers;

    //The bonus multipliers for number purchased
    private GeneratorBonus[] m_bonusMultipliers;

    //The base cost of the generator
    private NumberWithModifier baseCost;

    // Use this for initialization
    void Start ()
    {
        baseCost = new NumberWithModifier(m_baseNumber, m_baseExponent);
        m_resourceName.text = this.name;
        CalculateMultipliers();

    }

    /// <summary>
    /// Setup the initial multipliers for this generator
    /// </summary>
    /// <returns>The total multiplier for this generator</returns>
    public int CalculateMultipliers()
    {
        m_multipliers = 1;
        m_bonusMultipliers = new GeneratorBonus[] { new GeneratorBonus(25, 1),
                                                  new GeneratorBonus(50, 2),
                                                  new GeneratorBonus(100, 2),
                                                  new GeneratorBonus(250, 5),
                                                  new GeneratorBonus(500, 7),
                                                  new GeneratorBonus(1000, 10),
        };

        //Determine what multiplier this generator has
        foreach (GeneratorBonus genBonus in m_bonusMultipliers)
        {
            if (m_numResources >= genBonus.resourcesToComplete)
            {
                m_multipliers += genBonus.multiplier;
            }
        }

        return m_multipliers;
    }

    // Update is called once per frame
    void Update ()
    {
        NumberWithModifier cost = CalculateBuyAmount(ProcessGameLogic.GetBulkPurchaseAmount());
        m_buyButton.interactable = ProcessGameLogic.GetTotalMoney() > cost;
        
        //Determine a better way to do this. Don't need to set text if it didn't change
        m_price.text = NumberWithModifier.ToString(cost);
        m_totalResources.text = m_numResources.ToString();
	}

    /// <summary>
    /// Gets the total income that this generator is producing
    /// </summary>
    /// <returns>The total income being generated</returns>
    public NumberWithModifier GetProduction()
    {
        NumberWithModifier newProduction = new NumberWithModifier(0, 0);
        int exponent = 0;
        float number = m_production;
        while (number > 1000.0f)
        {
            number /= 1000.0f;
            exponent++;
        }
        
        newProduction.number = number;
        newProduction.exponent = exponent;
        return newProduction * (float)m_numResources * CalculateMultipliers();
    }

    /// <summary>
    /// Gets the number of resources owned
    /// </summary>
    /// <returns> The number of resources owned </returns>
    public int GetNumResources()
    {
        return m_numResources;
    }

    /// <summary>
    /// Calculate the cost to buy the provided number of generators
    /// </summary>
    /// <param name="numToBuy"> The amount we are attempting to purchase set by the bulk buy button</param>
    /// <returns> The total cost to buy </returns>
    private NumberWithModifier CalculateBuyAmount(int numToBuy)
    {
        NumberWithModifier currCost = baseCost * (Mathf.Pow(m_growthRate, m_numResources));
        //If we are only trying to buy one, just return the cost
        if (numToBuy == 1)
        {
            return currCost;
        }
        //Calculate the total cost if the number we want to buy
        else
        {
            NumberWithModifier bulkCost = (currCost * (Mathf.Pow(m_growthRate, numToBuy - 1))) / 0.15f;
            return bulkCost;
        }
    }

    /// <summary>
    /// Purchase the desired amount of generators
    /// </summary>
    public void BuyAmount ()
    {
        BuyAmount(ProcessGameLogic.GetBulkPurchaseAmount());
        m_uiManager.CheckForUpdatedImages(Resources.FindObjectsOfTypeAll<Generator>());
    }

    /// <summary>
    /// Buy the total amount passed in from the bulk buy button
    /// </summary>
    /// <param name="numToBuy"></param>
    private void BuyAmount(int numToBuy)
    {
        m_buyButton.interactable = false;
        NumberWithModifier cost = CalculateBuyAmount(numToBuy);
        ProcessGameLogic.DeductBalance(cost);
        m_numResources += numToBuy;
        //Since we bought more, recalculate the multipliers and see if we broke a threshold
        CalculateMultipliers();
    }

    /// <summary>
    /// Sets the number of resources we own
    /// </summary>
    /// <param name="numResources"> The number of resources we own </param>
    public void SetNumResources(int numResources)
    {
        this.m_numResources = numResources;
    }

    /// <summary>
    /// Reset the multipliers for this generator. Used if we prestige.
    /// </summary>
    public void ResetMultiplier()
    {
        m_multipliers = 1;
    }





}
