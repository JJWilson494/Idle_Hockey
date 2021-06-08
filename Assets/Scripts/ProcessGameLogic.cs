using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class ProcessGameLogic : MonoBehaviour, IUnityAdsListener
{
    //The total money to display. TODO: Change to m_
    [SerializeField]
    private TextMeshProUGUI totalMoneyText;

    //The income per second text to display. TODO: Change to m_
    [SerializeField]
    private TextMeshProUGUI incomePerSecondText;

    //The offline time to display when the user starts. TODO: Change to m_
    [SerializeField]
    private TextMeshProUGUI offlineTimeText;

    //The offline income the player received. TODO: Change to m_
    [SerializeField]
    private TextMeshProUGUI offlineIncomeText;

    //The text display for the premium currency. TODO: Change to m_
    [SerializeField]
    private TextMeshProUGUI premiumCurrencyText;

    //Button for adding premium currency when an add is watched. TODO: Change to m_
    [SerializeField]
    private Button addPremiumCurrencyButton;

    //Button used for time skips. TODO: Change to m_
    [SerializeField]
    private Button SkipButton;

    //The text for the remaining ads. TODO: Change to m_
    [SerializeField]
    private TextMeshProUGUI adCountText;

    //The image for the ad count. TODO: Change to m_
    [SerializeField]
    private Image adCountImage;

    //Button for prestiging. TODO: Change to m_
    [SerializeField]
    private Button skillUpButton;

    //Total money we currently have. TODO: Change to m_
    private static NumberWithModifier totalMoney;

    //Income we are currently generating per second. TODO: Change to m_
    private static NumberWithModifier incomePerSecond;

    //Determine if we should generate income currently. TODO: Change to m_
    private bool shouldGenerateIncome;

    //List of all current income generators TODO: Change to m_
    private Generator[] allGenerators;

    //The number for how much we want to bulk purchase. TODO: Change to m_
    private static int bulkPurchaseAmount;

    //The current date time for offline income comparison. TODO: Change to m_
    private DateTime currDate;

    //The previous time, used for loading and offline time comparison. TODO: Change to m_
    private DateTime prevDate;

    //Determination if we are paused. TODO: Change to m_
    private bool isPaused;

    //How quickly we generate income. TODO: Change to m_
    private int incomeGenerationTime = 1;

    //Minimum time in seconds we need to be offline for. TODO: Change to m_
    private int minOfflineTime = 1;

    //Maximum amount of time we can be offline for. TODO: Change to m_
    private int maxOfflineTime = 604800;

    //Current overall multipliers. TODO: Change to m_
    private static int multipliers;

    //Current prestige level multiplier. TODO: Change to m_
    private static float prestigeMultipler;

    //Number of premium currency. TODO: Change to m_
    private static int premiumCurrency;

    //Current daily ad count. TODO: Change to m_
    private static int dailyAdCount;

    //Ad type used for showing ads. TODO: Change to m_
    private string adType;

    //Access to our UI manager for updating displays. TODO: Change to m_
    [SerializeField]
    private UIManager uiManager;

    //App ID for display ads
#if UNITY_IOS
    private string gameId = "3258764";
#elif UNITY_ANDROID
    private string gameId = "3258765";
#endif

    //Test mode flag for ad display
    private bool testMode = true;

    //ID for our ad display
    private string myPlacementId = "AdReward";

    // Use this for initialization
    void Start()
    {
        adType = "";
        DataManager.LoadData();
        allGenerators = Resources.FindObjectsOfTypeAll<Generator>();
        uiManager.CheckForUpdatedImages(allGenerators);
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        incomePerSecond = CalculateIncomePerSecond();
        HandleElapsedOfflineTime();
        totalMoneyText.text = "Income: " + NumberWithModifier.ToString(totalMoney);
        incomePerSecondText.text = "Per Second: " + NumberWithModifier.ToString(incomePerSecond);
        shouldGenerateIncome = true;
        isPaused = false;
        bulkPurchaseAmount = 1;
        if (dailyAdCount <= 0)
        {
            UIManager.HideUIObject(UIManager.ALERT_IMAGE);
        }
        else
        {
            UIManager.ShowUIObject(UIManager.ALERT_IMAGE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        incomePerSecond = CalculateIncomePerSecond();
        if (multipliers > 1)
        {
            incomePerSecondText.text = "Per Second: " + NumberWithModifier.ToString(incomePerSecond) + " (x" + multipliers + ")";
        }
        else
        {
            incomePerSecondText.text = "Per Second: " + NumberWithModifier.ToString(incomePerSecond);
        }
        totalMoneyText.text = "Income: " + NumberWithModifier.ToString(totalMoney);
        if (shouldGenerateIncome && !isPaused)
        {
            StartCoroutine(GenerateIncome());
        }
        premiumCurrencyText.text = "" + premiumCurrency;
        adCountText.text = "" + dailyAdCount;
        if (dailyAdCount <= 0)
        {
            UIManager.HideUIObject(UIManager.ALERT_IMAGE);
        }
        else
        {
            UIManager.ShowUIObject(UIManager.ALERT_IMAGE);
        }
        skillUpButton.interactable = DataManager.GetCurrentSkillUpRequirement().GetSkillUpSatisfied();
    }

    /// <summary>
    /// Check if we have available ads
    /// </summary>
    /// <returns></returns>
    private bool CheckForAvailableAds()
    {
        return dailyAdCount > 0;
    }

    /// <summary>
    /// Add income for things like timeskip or normal income generation
    /// </summary>
    /// <param name="income">Income to add</param>
    private void AddIncome( NumberWithModifier income )
    {
        totalMoney += income;
        StatisticsManager.AddToLifetimeIncome(income);
    }

    /// <summary>
    /// Function for adding income per second. 
    /// </summary>
    /// <returns>1 second pause</returns>
    IEnumerator GenerateIncome()
    {
        shouldGenerateIncome = false;
        AddIncome(incomePerSecond);
        yield return new WaitForSecondsRealtime(incomeGenerationTime);
        shouldGenerateIncome = true;
    }

    /// <summary>
    /// Get the income per second
    /// </summary>
    /// <returns>The income per second</returns>
    public static NumberWithModifier GetIncomePerSecond()
    {
        return incomePerSecond;
    }

    /// <summary>
    /// Exposed function for adding income. TODO: investigate if this is still necessary
    /// </summary>
    /// <param name="income">The income to add</param>
    public static void AddToIncome(NumberWithModifier income)
    {
        totalMoney += income;
        StatisticsManager.AddToLifetimeIncome(income);
    }

    /// <summary>
    /// Calculate the income per second
    /// </summary>
    /// <returns>Calculate the current income per second</returns>
    private NumberWithModifier CalculateIncomePerSecond()
    {
        NumberWithModifier totalIncome = new NumberWithModifier(0, 0);
        foreach (Generator generator in allGenerators)
        {
            if (generator.enabled && generator.GetNumResources() > 0)
            {
                totalIncome += generator.GetProduction();
            }  
        }
        totalIncome = totalIncome * multipliers * prestigeMultipler;
        return totalIncome;
    }

    /// <summary>
    /// Gets the total money we have
    /// </summary>
    /// <returns>The total money</returns>
    public static NumberWithModifier GetTotalMoney()
    {
        return totalMoney;
    }

    /// <summary>
    /// Deduct the balance after we purchase something. 
    /// </summary>
    /// <param name="cost">The cost of the purchase. </param>
    public static void DeductBalance(NumberWithModifier cost)
    {
        totalMoney = totalMoney - cost;
    }

    /// <summary>
    /// Gets the current bulk purchase amount. 
    /// </summary>
    /// <returns>The bulk purchase amount</returns>
    public static int GetBulkPurchaseAmount()
    {
        return bulkPurchaseAmount;
    }

    /// <summary>
    /// Sets the bulk purchase amount
    /// </summary>
    /// <param name="bulk">The bulk purchase amount</param>
    public static void SetBulkPurchaseAmount(int bulk)
    {
        bulkPurchaseAmount = bulk;
    }

    /// <summary>
    /// Sets the total money we own
    /// </summary>
    /// <param name="num">The total money</param>
    public static void SetTotalMoney(NumberWithModifier num)
    {
        totalMoney = num;
    }

    /// <summary>
    /// Sets the daily ad count
    /// </summary>
    /// <param name="adCount">The daily ad count</param>
    public static void SetDailyAdCount(int adCount)
    {
        dailyAdCount = adCount;
    }

    /// <summary>
    /// Gets the daily ad count
    /// </summary>
    /// <returns>The daily ad count</returns>
    public static int GetDailyAdAcount()
    {
        return dailyAdCount;
    }

    /// <summary>
    /// Calculates the total offline time based on the previous date and current date
    /// </summary>
    private void HandleElapsedOfflineTime()
    {
        currDate = System.DateTime.Now;

        if (File.Exists(Application.persistentDataPath + "/playerData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            prevDate = DateTime.FromBinary(Convert.ToInt64(data.lastLogin));
            TimeSpan offlineTime = currDate.Subtract(prevDate);

            double offlineSeconds = offlineTime.TotalSeconds;
            if (offlineSeconds < minOfflineTime)
            {
                return;
            }
            if (offlineSeconds > maxOfflineTime)
            {
                offlineSeconds = maxOfflineTime;
            }

            double offlineMultiplierSeconds = 0;
            NumberWithModifier offlineIncome = new NumberWithModifier(0, 0);

            //Determine if only part of our offline time contains the multiplier time
            DateTime multiplierEndTime = DataManager.GetMultiplierEndTime();
                if (DateTime.Compare(multiplierEndTime, prevDate) > 0)
                {
                    if (DateTime.Compare(multiplierEndTime, currDate) > 0)
                    {
                    offlineMultiplierSeconds = offlineSeconds;
                        offlineSeconds = 0;
                        offlineIncome = incomePerSecond * (float)offlineMultiplierSeconds;
                    }
                    else
                    {
                    TimeSpan multiplierSpan = multiplierEndTime.Subtract(prevDate);
                        offlineMultiplierSeconds = multiplierSpan.TotalSeconds;
                        offlineSeconds -= offlineMultiplierSeconds;
                        offlineIncome = (incomePerSecond * (float)offlineSeconds) + (incomePerSecond * 3.0f * (float)offlineMultiplierSeconds);

                    }

                }
                else
                {
                offlineIncome = incomePerSecond * (float)offlineSeconds;

                }
            
            offlineIncomeText.text = NumberWithModifier.ToString(offlineIncome);
            String offlineText = "You were offline for\n ";

            offlineText += offlineTime.Days.ToString("00") + ":"
                + offlineTime.Hours.ToString("00") + ":"
                + offlineTime.Minutes.ToString("00") + ":"
                + offlineTime.Seconds.ToString("00");

            offlineTimeText.text = offlineText;
            UIManager.ShowUIObject(UIManager.OFFLINE_PANEL_UI);
            AddIncome(offlineIncome);
            DataManager.ForceSaveData();
        }
    }

    /// <summary>
    /// Save on quit
    /// </summary>
    void OnApplicationQuit()
    {
        DataManager.ForceSaveData();
    }

    /// <summary>
    /// On app switch away, save and pause
    /// </summary>
    /// <param name="hasFocus">Flag for if the application has focus</param>
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            isPaused = true;
            DataManager.ForceSaveData();
        }
        else
        {
            isPaused = false;
            HandleElapsedOfflineTime();
        }
    }
    //TODO: Move to default close gameobject
    public void CloseOfflinePanel()
    {
        UIManager.HideUIObject(UIManager.OFFLINE_PANEL_UI);
    }
    //TODO: Move to default close gameobject

    public void OpenStatsPanel()
    {
        UIManager.ShowUIObject(UIManager.STATISTICS_PANEL_UI);
    }
    //TODO: Move to default close gameobject

    public void CloseStatsPanel()
    {
        UIManager.HideUIObject(UIManager.STATISTICS_PANEL_UI);
    }
    //TODO: Move to default close gameobject

    public void OpenShopPanel()
    {
        UIManager.ShowUIObject(UIManager.SHOP_PANEL_UI);
    }
    //TODO: Move to default close gameobject

    public void CloseShopPanel()
    {
        UIManager.HideUIObject(UIManager.SHOP_PANEL_UI);
    }

    /// <summary>
    /// Sets the ad multiplier value
    /// </summary>
    /// <param name="multiplier">The multiplier value</param>
    public static void SetAdMultiplier( int multiplier )
    {
        multipliers = multiplier;
    }

    /// <summary>
    /// Sets the prestige multiplier value
    /// </summary>
    /// <param name="multiplier">The multiplier value</param>
    public static void SetPrestigeMultiplier(float multiplier)
    {
        prestigeMultipler = multiplier;
    }

    /// <summary>
    /// Set the type of ad we are watching
    /// </summary>
    public void SkipTime()
    {
        if (Advertisement.IsReady(myPlacementId))
        {
            adType = "TimeSkip";
            Advertisement.Show();
        }
       
    }

    /// <summary>
    /// Set the type of ad we are watching
    /// </summary>
    public void AddHourMultipler()
    {
        if (Advertisement.IsReady(myPlacementId))
        {
            adType = "HourMultiplier";
            Advertisement.Show();
        }
    }

    /// <summary>
    /// Prestige the game
    /// </summary>
    public void Prestige()
    {
        DataManager.Prestige();
    }

    /// <summary>
    /// Add trophies
    /// </summary>
    /// <param name="currency">The amount of currency to add</param>
    public void AddPremiumCurrency(int currency)
    {
        premiumCurrency += currency;
    }

    /// <summary>
    /// If we watch the ad, add 3 currency.
    /// </summary>
    public void ButtonAddCurrency()
    {
        AddPremiumCurrency(3);
    }

    /// <summary>
    /// Set the premium currency count
    /// </summary>
    /// <param name="currency">The amount of currency</param>
    public static void SetPremiumCurrency(int currency)
    {
        premiumCurrency = currency;
    }

    /// <summary>
    /// Gets the premium currency amount
    /// </summary>
    /// <returns>The amount of tropies</returns>
    public static int GetPremiumCurrency()
    {
        return premiumCurrency;
    }

    //TODO: Move to default close gameobject
    public void ShowInformationWindow()
    {
        UIManager.ShowUIObject(UIManager.INFORMATION_UI);
    }

    /// <summary>
    /// Set ad for daily currency
    /// </summary>
    public void WatchPremiumAd()
    {
        if (dailyAdCount > 0)
        {
            adType = "DailyCurrencyAdd";
            if (Advertisement.IsReady(myPlacementId))
            {
                Advertisement.Show();
            }
        }

        if (dailyAdCount <= 0)
        {
            UIManager.HideUIObject(UIManager.ALERT_IMAGE);
        }
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status
        if (showResult == ShowResult.Finished)
        {
            switch(adType)
            {
                case "DailyCurrencyAdd":
                    dailyAdCount--;
                    AddPremiumCurrency(3);
                    break;
                case "TimeSkip":
                    NumberWithModifier timeSkipIncome = CalculateIncomePerSecond() * 1000.0f;
                    AddIncome(timeSkipIncome);
                    break;
                case "HourMultiplier":
                    MultiplierManager.AddHourOfMultiplier();
                    break;
            }
        }
        else if (showResult == ShowResult.Skipped)
        {
            Debug.LogWarning("User skipped ad");
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("Error showing ads. ");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad
        if (placementId == myPlacementId)
        {
            Advertisement.Show(myPlacementId);
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    //TODO: Move to default close gameobject
    public void ShowMiniGame()
    {
        UIManager.ShowUIObject(UIManager.MINIGAME_UI);
    }

}
