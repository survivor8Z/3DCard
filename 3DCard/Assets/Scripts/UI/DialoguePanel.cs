using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public struct DialogueData
{
    public string text;
    public string soundEffectName;
    public DialogueData(string text, string soundEffectName)
    {
        this.text = text;
        this.soundEffectName = soundEffectName;
    }
}

public class DialoguePanel : BasePanel
{   
    public void GetDialogueData(DialogueData dialogueData)
    {
        GetControl<TextMeshProUGUI>("DialogueText").text = dialogueData.text;
    }
    public override void ShowMe()
    {
        gameObject.SetActive(true);
    }
    public override void HideMe()
    {
        gameObject.SetActive(false);
    }

    
}
