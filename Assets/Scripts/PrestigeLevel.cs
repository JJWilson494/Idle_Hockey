using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to hold the information for the prestige level
/// </summary>
public class PrestigeLevel {

	//The multipler we get for this level
	private float m_prestigeMultiplier;

	//The name of the leve
	private string m_prestigeLevelName;

	//The requirement for leveling up
	private SkillUpRequirement m_skillUpRequirement;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="multiplier">The multiplier for this level</param>
	/// <param name="name">The name of the level</param>
	/// <param name="levelUp">The requirements for leveling up</param>
	public PrestigeLevel(float multiplier, string name, SkillUpRequirement levelUp)
    {
		m_prestigeMultiplier = multiplier;
		m_prestigeLevelName = name;
		m_skillUpRequirement = levelUp;
    }

	/// <summary>
	/// Gets the multiplier for this level
	/// </summary>
	/// <returns>The multiplier</returns>
	public float GetPrestigeMultipler()
    {
		return m_prestigeMultiplier;
    }

	/// <summary>
	/// Gets the name of the prestige level
	/// </summary>
	/// <returns>The name of the level</returns>
	public string GetPrestigeLevelName()
    {
		return m_prestigeLevelName;
    }

	/// <summary>
	/// Gets the requirement for this prestige
	/// </summary>
	/// <returns>The requirement to prestige</returns>
	public SkillUpRequirement GetPrestigeSkillUpRequirement()
    {
		return m_skillUpRequirement;
    }
	
}
