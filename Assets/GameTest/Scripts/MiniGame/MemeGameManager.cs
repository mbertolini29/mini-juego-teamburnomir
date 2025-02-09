using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doublsb.Dialog;

namespace Test
{
    public class MemeGameManager : MonoBehaviour
    {
        [SerializeField] private List<MemeOption> memeOptions;
        [SerializeField] private Image memeImage1, memeImage2, memeImage3;
        [SerializeField] private Text memeText1, memeText2, memeText3;
        //[SerializeField] private CharacterReaction characterReaction;
        [SerializeField] private GameObject memeGamePanel;
        [SerializeField] private DialogManager dialogManager;

        private MemeOption selectedMeme;

        private void Start()
        {
            memeGamePanel.SetActive(false);
        }

        public void StartMemeGame()
        {
            memeGamePanel.SetActive(true);
            LoadMemes();
        }

        private void LoadMemes()
        {
            if(memeOptions == null || memeOptions.Count < 3)
            {
                Debug.LogError("No hay suficientes memes en la lista.");
                return;
            }

            //shuffle = mezclar
            List<MemeOption> shuffledMemes = new List<MemeOption>(memeOptions);
            shuffledMemes.Shuffle(); //te devuelve los memes mezclados.

            AssignMemeUI(memeImage1, memeText1, shuffledMemes[0]);
            AssignMemeUI(memeImage2, memeText2, shuffledMemes[1]);
            AssignMemeUI(memeImage3, memeText3, shuffledMemes[2]);
        }

        private void AssignMemeUI(Image memeImage, Text memeText, MemeOption memeOption)
        {
            if (memeImage != null) memeImage.sprite = memeOption.MemeSprite;
            if (memeImage != null) memeText.text = memeOption.MemeText;
        }
            
        public void SelectMeme(int index)
        {
            if (index < 0 || index >= memeOptions.Count)
            {
                Debug.LogError("Índice de meme invalido.");
                return;
            }

            selectedMeme = memeOptions[index];
            //characterReaction.ReactToMeme(selectedMeme.GraceLevel);
            EndMemeGame();
        }

        private void EndMemeGame()
        {
            memeGamePanel.SetActive(false);
            //dialogManager.ContinueDialogAfterMeme(selectedMeme.GraceLevel);
        }
    }
}
