using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using TMPro;

namespace Test
{
    public class DialogController : MonoBehaviour
    {
        [SerializeField] private DialogManager dialogManager;

        [SerializeField] private GameObject miniGamePanel;

        [SerializeField] private List<MemeOption> miniGameMemes;

        private Queue<DialogData> dialogQueue = new Queue<DialogData>();
        private bool isConversationActive = false;
        private bool isMiniGameActive = false;

        private void OnEnable()
        {
            if(dialogManager != null)
                dialogManager.OnMemeSelected += OnMiniGameFinished;            
        }

        private void OnDisable()
        {
            if (dialogManager != null)
                dialogManager.OnMemeSelected -= OnMiniGameFinished;            
        }

        private void Start()
        {
            StartConversation();
        }

        private void Update()
        {
            if (!isMiniGameActive && !dialogManager.isTyping)
            {
                if (isConversationActive && (Input.GetKeyDown(KeyCode.Return) ||
                                             Input.GetMouseButtonDown(0)))
                {
                    dialogManager.isTyping = true;
                    ShowNextDialog();
                }
            }            
        }

        private void StartConversation()
        {
            List<DialogData> dialogs = new List<DialogData>();
            
            dialogs.Add(new DialogData("Hola, como estas?", "Player 1"));
            dialogs.Add(new DialogData("Amigo, no sabés lo que me paso hoy.", "Player 2"));
            dialogs.Add(new DialogData("Uy, contamé", "Player 1"));
            dialogs.Add(new DialogData("Estaba caminando por av corriente y de repente..", "Player 2", () => MiniGame()));

            //dialogManager            

            dialogs.Add(new DialogData("jajaja", "Player 1"));

            foreach (var dialog in dialogs)
            {
                dialogQueue.Enqueue(dialog);
            }

            isConversationActive = true;
            ShowNextDialog();
        }

        private void ShowNextDialog()
        {
            if (dialogQueue.Count == 0)
            {
                isConversationActive = false;
                dialogManager.Hide();
                return;
            }

            var currentDialog = dialogQueue.Dequeue();
            dialogManager.Show(currentDialog);
        }   

        private void MiniGame()
        {
            if (dialogManager.state == State.Deactivate) return;            
            dialogManager.Hide();

            isMiniGameActive = true;

            // configurar los memes?
            SetupMiniGame();

            // activa el panel de mini juego.
            miniGamePanel.SetActive(true);
        }

        public void OnMiniGameFinished(MemeQuality quality)
        {
            miniGamePanel.SetActive(false);
            dialogManager.state = State.Active;

            isMiniGameActive = false;

            ShowNextDialog();
        }

        private void SetupMiniGame()
        {
            foreach (var meme in miniGameMemes)
            {
                //vincula cada meme.
                meme.OnMemeSelected -= dialogManager.MemeSelected;
                meme.OnMemeSelected += dialogManager.MemeSelected;
            }
        }

    }
}