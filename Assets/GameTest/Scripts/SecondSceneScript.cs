using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class SecondSceneScript : MonoBehaviour
{

    public DialogManager DialogManager;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("Here you can create any mini-game you can think of and make me react to it", "Li"));

        dialogTexts.Add(new DialogData("I doesn't have to be complex nor long, just some short and simple stuff.", "Li"));        

        dialogTexts.Add(new DialogData("Use my sprite, leave me on a side of the screen, make a few animations", "Li"));

        dialogTexts.Add(new DialogData("And depending the actions of the mini-game change mi animation state, move me or do whatever crazy stuff you can think of!", "Li"));

        dialogTexts.Add(new DialogData("After the player finishes the game, make me say something and move to the ThirdScene.", "Li"));
        
        DialogManager.Show(dialogTexts);
    }
}
