using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for storing information related to prestiging
/// </summary>
public class SkillUpRequirement {

    //The requirements for the prestige
    public SkillUpInternal[] skillUpPieces;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SkillUpRequirement()
    {

    }

    /// <summary>
    /// Parameterized Constructor
    /// </summary>
    /// <param name="requirements">The requirements for this prestige</param>
    public SkillUpRequirement(SkillUpInternal[] requirements)
    {
        skillUpPieces = requirements;
    }

    /// <summary>
    /// Internal class for determining the name and amount of resource required to prestige
    /// </summary>
    public class SkillUpInternal
    {
        //The number of resources necessary
        public int requiredResourceCount;

        //The name of the generator
        public string generatorName;

        /// <summary>
        /// Paramerized Constructor
        /// </summary>
        /// <param name="genName">The name of the generator</param>
        /// <param name="count">The total required number of generators</param>
        public SkillUpInternal(string genName, int count)
        {
            generatorName = genName;
            requiredResourceCount = count;
        }
    }

    /// <summary>
    /// Determine if each internal requirement is met
    /// </summary>
    /// <returns>True if the requirement is met</returns>
    public bool GetSkillUpSatisfied()
    {
        bool satisfied = true;
        foreach (SkillUpInternal piece in skillUpPieces)
        {
            Generator gen = GameObject.Find(piece.generatorName).GetComponent<Generator>();
            satisfied = satisfied && gen.GetNumResources() >= piece.requiredResourceCount;
        }

        return satisfied;
    }
}
