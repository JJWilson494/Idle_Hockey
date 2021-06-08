using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for keeping track of the resource to multiplier ratio
/// </summary>
public class GeneratorBonus {

    //The resources needed to satisfy the condition
    public int resourcesToComplete;

    //The multiplier received when the condition is satisfied
    public int multiplier;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="resources"> The number of required resources </param>
    /// <param name="mult"> The multiplier </param>
    public GeneratorBonus(int resources, int mult)
    {
        resourcesToComplete = resources;
        multiplier = mult;
    }
}
