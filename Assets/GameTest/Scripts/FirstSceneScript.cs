using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class FirstSceneScript : MonoBehaviour
{
    public DialogManager DialogManager;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Li.", "Li"));

        dialogTexts.Add(new DialogData("Let's start this test!", "Li"));

        dialogTexts.Add(new DialogData("The idea is to create something very simple, make me react to the text and animate me in a few different ways.", "Li"));

        dialogTexts.Add(new DialogData("Remember that you can change my sprite or background if you choose so, you can even go 3D if you're more experienced with that!", "Li"));

        dialogTexts.Add(new DialogData("Anyways... Let's move on!", "Li"));

        dialogTexts.Add(new DialogData("Create a branching option where you have to choose between 3 possible answers to give me.", "Li"));

        dialogTexts.Add(new DialogData("Once you create the options and pick one of them I'll say: ", "Li"));

        dialogTexts.Add(new DialogData("Yo choose option A!", "Li"));

        dialogTexts.Add(new DialogData("Or...", "Li"));

        dialogTexts.Add(new DialogData("Yo choose option B!", "Li"));

        dialogTexts.Add(new DialogData("Or...", "Li"));

        dialogTexts.Add(new DialogData("Yo choose option C!", "Li"));

        dialogTexts.Add(new DialogData("After this, you'll send me to the SecondScene!", "Li"));

        dialogTexts.Add(new DialogData("Where you'll have to create a VERY simple mini game and make me react to it.", "Li"));

        DialogManager.Show(dialogTexts);
    }
}
