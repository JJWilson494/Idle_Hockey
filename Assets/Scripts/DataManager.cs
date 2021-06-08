using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Class managing the logic for daily rewards, saving and loading the game, and in game requirements such as prestiging
/// </summary>
public class DataManager : MonoBehaviour 
{
    //The current prestige level of the player
    private static int m_prestigeLevel;

    //Flag for indicating if we should auto save
    private bool m_shouldSaveGame;

    //Flag for indicating if we should check the daily login
    private bool m_shouldCheckDailyLogin;

    //Array of all income generators
    private static Generator[] m_allGenerators;

    //The end time of the ad multiplier. Used for determining how much income should be received if the 
    //player closes the app with the multiplier enabled.
    private static DateTime m_multiplierEndTime;

    //Flag indicating if they have received the daily rewards for login.
    private static bool m_receivedDailyRewards;

    //Time for indicating when the player is next eligible for the login reward
    private static DateTime m_nextRewardTime;

    //The username of the player
    private static string m_username;

    //Highschool prestige level
    private static readonly PrestigeLevel m_highSchool = new PrestigeLevel(1.0f, "Highschool", new SkillUpRequirement(
            new SkillUpRequirement.SkillUpInternal[]
            {
                new SkillUpRequirement.SkillUpInternal("Fans", 1),
            }));
    //College prestige level
    private static readonly PrestigeLevel m_college = new PrestigeLevel(1.5f, "Highschool", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 5),
        }));
    //Juniors prestige level
    private static readonly PrestigeLevel juniors = new PrestigeLevel(2.0f, "Juniors", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 20),
        }));
    //Major Juniors prestige level
    private static readonly PrestigeLevel majorJuniors = new PrestigeLevel(2.5f, "Major Junior", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 50),
        }));
    //FHL prestige level
    private static readonly PrestigeLevel FHL = new PrestigeLevel(3.0f, "FHL", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 75),
        }));
    //ECHL prestige level
    private static readonly PrestigeLevel ECHL = new PrestigeLevel(4.0f, "ECHL", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 100),
        }));
    //AHL prestige level
    private static readonly PrestigeLevel AHL = new PrestigeLevel(5.0f, "AHL", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 200),
        }));
    //NHL prestige level
    private static readonly PrestigeLevel NHL = new PrestigeLevel(7.0f, "NHL", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 500),
        }));
    //Olympics prestige level
    private static readonly PrestigeLevel olympics = new PrestigeLevel(10.0f, "Olympics", new SkillUpRequirement(
        new SkillUpRequirement.SkillUpInternal[]
        {
                new SkillUpRequirement.SkillUpInternal("Fans", 99999),
        }));
    //Array containing all possible prestige levels
    private static readonly PrestigeLevel[] m_prestigeLevels  = new PrestigeLevel[] { m_highSchool, m_college, juniors, majorJuniors, FHL, ECHL, AHL, NHL, olympics };

    /// <summary>
    /// Function called every frame
    /// </summary>
    void Update()
    {
        //Determine if enough time has passed that we should save the game
        if (m_shouldSaveGame)
        {
            StartCoroutine(Save());
        }
        //Determine if enough time has passed that we should check the daily login
        if (m_shouldCheckDailyLogin)
        {
            StartCoroutine(CheckDailyLogin());
        }
    }

    /// <summary>
    /// Gets the end time of the ad income multipler
    /// </summary>
    /// <returns> The date time associated with the end of the ad multiplier</returns>
    public static DateTime GetMultiplierEndTime()
    {
        return m_multiplierEndTime;
    }

    /// <summary>
    /// Sets the username of the player
    /// </summary>
    /// <param name="name"> The current username </param>
    public static void SetUsername(string name)
    {
        m_username = name;
    }

    /// <summary>
    /// Gets the user name of the player
    /// </summary>
    /// <returns> The username of the player </returns>
    public static string GetUsername()
    {
        return m_username;
    }

    /// <summary>
    /// Loads the player save file and updates necessary information
    /// </summary>
    public static void LoadData()
    {
        Debug.Log("Loading Data");
        //If we have a save file
        if(File.Exists(Application.persistentDataPath + "/playerData.dat"))
        {
            Debug.Log("Have save file");

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);

            //Parse out the end time for the multiplier so we can give out the proper amount of income
            DateTime endTime = DateTime.FromBinary(Convert.ToInt64(data.lastLogin));
            //If now is past the end time, we need to set the multiplier to 1
            if (DateTime.Compare(endTime, System.DateTime.Now) <= 0)
            {
                ProcessGameLogic.SetAdMultiplier(1);
            }
            //If we haven't hit the end time of the multiplier, it is still active, so we enable it.
            else
            {
                ProcessGameLogic.SetAdMultiplier(3);
            }

            //Split the delimited string to get the amount of each generator we currently have
            string allGeneratorString = data.allGenerators;
            string[] allGeneratorsSplit = allGeneratorString.Split(':');
            foreach (string gen in allGeneratorsSplit)
            {
                string[] genPieces = gen.Split(',');
                m_allGenerators = Resources.FindObjectsOfTypeAll<Generator>();
                Debug.Log("Set all Gens");

                //Set the number of generators we own from the save file.
                foreach (Generator generator in m_allGenerators)
                {
                    if (generator.name == genPieces[0])
                    {
                        generator.SetNumResources(Int32.Parse(genPieces[1]));
                        break;
                    }
                }
            }
            //Parse out the total income and apply it.
            string totalMoneyString = data.totalMoney;
            string[] totalMoneyPieces = totalMoneyString.Split(':');
            ProcessGameLogic.SetTotalMoney(new NumberWithModifier((float)Double.Parse(totalMoneyPieces[0]), Int32.Parse(totalMoneyPieces[1])));
            //Parse out the prestige level.
            m_prestigeLevel = data.prestigeLevel;
            if (m_prestigeLevel >= m_prestigeLevels.Length)
            {
                m_prestigeLevel = m_prestigeLevels.Length - 1;
            }
            ProcessGameLogic.SetPrestigeMultiplier(m_prestigeLevels[m_prestigeLevel].GetPrestigeMultipler());

            //Parse out the lifetime income earned and set
            string lifetimeMoneyString = data.lifetimeIncome;
            string[] lifetimeMoneyPieces = lifetimeMoneyString.Split(':');
            StatisticsManager.SetLifetimeIncome(new NumberWithModifier((float)Double.Parse(lifetimeMoneyPieces[0]), Int32.Parse(lifetimeMoneyPieces[1])));

            //If we still have a multiplier, set that end time
            m_multiplierEndTime = DateTime.FromBinary(Convert.ToInt64(data.multiplierEndTime));

            //Parse out the tropies or "premium currency"
            int premiumCurrency = data.premiumCurrency;

            ProcessGameLogic.SetPremiumCurrency(premiumCurrency);

            //Parse out the amount of daily ads accumulated.
            int premiumAds = data.premiumAdCount;
            ProcessGameLogic.SetDailyAdCount(premiumAds);

            //Parse out the flag determining if we need to give them the daily reward.
            m_receivedDailyRewards = data.receivedDailyRewards;

            //Parse out the next reward eligibility time
            m_nextRewardTime = DateTime.FromBinary(Convert.ToInt64(data.nextRewardTime));

            //Parse the username of the player
            m_username = data.username;

            //Close the file.
            file.Close();
        }
        //If we do not have a save file, this is the first time we are playing.
        else
        {
            //Initialize the ad multiplier
            ProcessGameLogic.SetAdMultiplier(1);
            m_allGenerators = Resources.FindObjectsOfTypeAll<Generator>();
            //Initialize 1 "Sticks" resource
            foreach (Generator generator in m_allGenerators)
            {
                if (generator.name == "Sticks")
                {
                    generator.SetNumResources(1);
                }
                else
                {
                    generator.SetNumResources(0);
                }
            }
            //Initialize first time values
            ProcessGameLogic.SetTotalMoney(new NumberWithModifier(0, 0));
            m_prestigeLevel = 0;
            ProcessGameLogic.SetPrestigeMultiplier(m_prestigeLevels[m_prestigeLevel].GetPrestigeMultipler());
            StatisticsManager.SetLifetimeIncome(new NumberWithModifier(0, 0));
            m_multiplierEndTime = DateTime.Now;
            ProcessGameLogic.SetPremiumCurrency(0);
            m_receivedDailyRewards = false;
            m_nextRewardTime = DateTime.Today;
            m_username = "User-" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
        }
    }

    /// <summary>
    /// Check for daily login rewards. If we have passed the next reward time, 
    /// grant the reward and set the next applicable reward to just after midnight tomorrow.
    /// </summary>
    /// <returns> 60 second timer </returns>
    private IEnumerator CheckDailyLogin()
    {
        m_shouldCheckDailyLogin = false;
        if (DateTime.Compare(DateTime.Now, m_nextRewardTime) >= 0)
        {
            m_nextRewardTime = DateTime.Today.AddDays(1);
            int dailyAds = ProcessGameLogic.GetDailyAdAcount();
            ProcessGameLogic.SetDailyAdCount(dailyAds + 3);
            m_receivedDailyRewards = true;
            //Ensure we save the data after applying this info
            ForceSaveData();
        }
        yield return new WaitForSecondsRealtime(60);
        m_shouldCheckDailyLogin = true;
    }

    /// <summary>
    /// Gets the current daily reward status
    /// </summary>
    /// <returns> Flag representing the current reward status</returns>
    public static bool GetDailyRewardStatus()
    {
        return m_receivedDailyRewards;
    }

    /// <summary>
    /// Sets the current reward status
    /// </summary>
    /// <param name="status"> The current status of the daily reward </param>
    public static void SetDailyRewardStatus(bool status)
    {
        m_receivedDailyRewards = status;
    }

    /// <summary>
    /// Saves the current player data
    /// </summary>
    /// <returns> 30 second timer </returns>
    IEnumerator Save()
    {
        m_shouldSaveGame = false;
        ForceSaveData();
        yield return new WaitForSecondsRealtime(30);
        m_shouldSaveGame = true;
    }

    /// <summary>
    /// Creates a save data object and serializes it to the filestream
    /// </summary>
    public static void ForceSaveData()
    {
        NumberWithModifier totalMoney = ProcessGameLogic.GetTotalMoney();

        //Create a double delimited string comprised of all the generators we have
        string allGeneratorsString = "";
        foreach (Generator gen in m_allGenerators)
        {
            allGeneratorsString += gen.name + "," + gen.GetNumResources() + ":";
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + "/playerData.dat");

        //Create a save data file to write out.
        SaveData data = new SaveData
        {
            allGenerators = allGeneratorsString,
            totalMoney = totalMoney.number + ":" + totalMoney.exponent,
            lastLogin = System.DateTime.Now.ToBinary().ToString(),
            lifetimeIncome = StatisticsManager.GetLifetimeIncome().number + ":" + StatisticsManager.GetLifetimeIncome().exponent,
            prestigeLevel = m_prestigeLevel,
            multiplierEndTime = MultiplierManager.GetMultiplierEndTime(),
            premiumCurrency = ProcessGameLogic.GetPremiumCurrency(),
            receivedDailyRewards = m_receivedDailyRewards,
            nextRewardTime = m_nextRewardTime.ToBinary().ToString(),
            premiumAdCount = ProcessGameLogic.GetDailyAdAcount(),
            username = m_username
        };
        //Serialize the data structure and close the file.
        bf.Serialize(fs, data);
        fs.Close();
    }


    /// <summary>
    /// Prestiges the player when the button is pressed
    /// </summary>
    public static void Prestige()
    {
        //If we hit max prestige, just return for now
        if (m_prestigeLevel + 1 >= m_prestigeLevels.Length)
        {
            return;
        }
        //If we can prestige
        else
        {
            //Increment the prestige level of the player and reset all resources and money
            m_prestigeLevel++;
            foreach (Generator gen in m_allGenerators)
            {
                if (gen.name == "Sticks")
                {
                    gen.SetNumResources(1);
                }
                else
                {
                    gen.SetNumResources(0);
                }
                gen.ResetMultiplier();
            }
            ProcessGameLogic.SetTotalMoney(new NumberWithModifier(0, 0));
            //Save the game after prestige
            ForceSaveData();
            ProcessGameLogic.SetPrestigeMultiplier(m_prestigeLevels[m_prestigeLevel].GetPrestigeMultipler());
        }
    }

    /// <summary>
    /// Returns the name of the current prestige level we are at
    /// </summary>
    /// <returns> The name of the prestige level</returns>
    public static string GetCurrentPrestigeName()
    {
        return m_prestigeLevels[m_prestigeLevel].GetPrestigeLevelName();
    }

    /// <summary>
    /// Returns the multiplier for the current prestige level we are at.
    /// </summary>
    /// <returns> The multiplier for the current prestige level</returns>
    public static float GetCurrentPrestigeMultiplier()
    {
        return m_prestigeLevels[m_prestigeLevel].GetPrestigeMultipler();
    }

    /// <summary>
    /// Returns the current requirement to prestige up
    /// </summary>
    /// <returns> The generator requirements for prestiging up</returns>
    public static SkillUpRequirement GetCurrentSkillUpRequirement()
    {
        if (m_prestigeLevel + 1 >= m_prestigeLevels.Length)
        {
            return new SkillUpRequirement( new SkillUpRequirement.SkillUpInternal[] { });
        }
        else
        {
            return m_prestigeLevels[m_prestigeLevel].GetPrestigeSkillUpRequirement();
        }
    }
}
