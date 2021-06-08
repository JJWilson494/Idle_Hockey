using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Object to serialize for data saving
/// </summary>
[Serializable]
public class SaveData
{
    public string totalMoney;
    public string allGenerators;
    public string lastLogin;
    public string lifetimeIncome;
    public int prestigeLevel;
    public string multiplierEndTime;
    public int premiumCurrency;
    public int premiumAdCount;
    public bool receivedDailyRewards;
    public string nextRewardTime;
    public string username;
    

}
