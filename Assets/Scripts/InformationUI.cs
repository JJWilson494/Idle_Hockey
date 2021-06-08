using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Backing script for the statistics pane.
/// </summary>
public class InformationUI : MonoBehaviour {

    //The dynamic requirement text for prestieging. 
    [SerializeField]
    private TextMeshProUGUI m_requirementText;

	void Update ()
    {
		if (isActiveAndEnabled)
        {
            UpdateRequirements();
        }
	}

    /// <summary>
    /// Set the display text for what is required to prestige
    /// </summary>
    void UpdateRequirements()
    {
        string updateText = "";
        if (DataManager.GetCurrentSkillUpRequirement().skillUpPieces.Length == 0)
        {
            m_requirementText.text = "Already at max skill!";
            return;
        }
        foreach (SkillUpRequirement.SkillUpInternal requirement in DataManager.GetCurrentSkillUpRequirement().skillUpPieces)
        {
            updateText += requirement.generatorName + ": " + requirement.requiredResourceCount + "\n";
        }
        m_requirementText.text = updateText;
    }

    /// <summary>
    /// Close the pane. 
    /// TODO: We can do this through the editor UI, change to use the default function
    /// </summary>
    public void CloseInformationPane()
    {
        UIManager.HideUIObject(UIManager.INFORMATION_UI);
    }
}
