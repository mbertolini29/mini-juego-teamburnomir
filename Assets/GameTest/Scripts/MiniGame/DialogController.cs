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

        private Dictionary<MemeQuality, DialogData> memeDialogs;

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
            // iniciamos los dialogos segun el meme seleccionado.
            memeDialogs = new Dictionary<MemeQuality, DialogData>
            {
                { MemeQuality.Happy, new DialogData ("JAJAJA, ese meme es genial!", "Player 1" ) },
                { MemeQuality.Normal, new DialogData ("Mmm, esta bueno pero no tan bueno.", "Player 1" ) },
                { MemeQuality.Sad, new DialogData ("Silencio incomodo..., bueno, ¿en qué estabamos?", "Player 1" ) }
            };

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
            
            //dialogs.Add(new DialogData("Hola, como estas?", "Player 1"));
            //dialogs.Add(new DialogData("Amigo, no sabés lo que me paso hoy.", "Player 2"));
            dialogs.Add(new DialogData("Uy, contamé", "Player 1"));
            dialogs.Add(new DialogData("Estaba caminando por av corriente y de repente..", "Player 2", () => MiniGame()));
  
            //dialogs.Add(new DialogData("JAJAJA, ese meme es genial!", "Player 1"));
            //dialogs.Add(new DialogData("Mm, esta bueno pero no tan bueno", "Player 1"));
            //dialogs.Add(new DialogData("silencio..., mm, bueno en que estabamos?", "Player 2"));

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

            // verifica si hay dialogo para el meme o usa por defecto.

            if(memeDialogs.TryGetValue(quality, out DialogData nextDialog))
            {
                dialogManager.Show(nextDialog);
            }
            else
            {
                dialogManager.Show(new DialogData("Bueno, sigamos...", "Player 2"));
            }

            isMiniGameActive = false;
            //ShowNextDialog();
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