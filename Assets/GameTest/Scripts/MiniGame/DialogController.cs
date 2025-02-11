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

        //private Dictionary<MemeQuality, List<DialogData>> memeDialogs;

        private Dictionary<MemeQuality, (List<DialogData> dialogs, string player1Emotion, string player2Emotion)> memeDialogs;

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
            memeDialogs = new Dictionary<MemeQuality, (List<DialogData>, string, string)>
            {
                { MemeQuality.Happy, (new List<DialogData>
                    {
                        new DialogData("JAJAJA, ese meme es genial!", "Player 1" ),
                        new DialogData("JAJAJAJA", "Player 2")
                    }, "Happy", "Happy")
                },                
                { MemeQuality.Normal, (new List<DialogData>
                    {
                        new DialogData("Mmm, esta bueno pero no tan bueno.", "Player 1" ),
                        new DialogData("Vimos mejores memes.", "Player 2" )
                    }, "Normal", "Normal")             
                },                
                { MemeQuality.Sad, (new List<DialogData>
                    {
                        new DialogData("Silencio incómodo...", "Player 1" ),
                        new DialogData("Bueno, ¿en qué estabamos?", "Player 2" )
                    }, "Sad", "Sad")
                }
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

            // configurar la imagen de los memes individualmente.
            SetupMiniGame();

            // activa el panel de mini juego.
            miniGamePanel.SetActive(true);
        }

        public void OnMiniGameFinished(MemeQuality quality)
        {
            miniGamePanel.SetActive(false);
            dialogManager.state = State.Active;
            isMiniGameActive = false;

            // verifica si hay dialogo para el meme o usa por defecto.
            if(memeDialogs.TryGetValue(quality, out var data))
            {
                List<DialogData> nextDialogs = data.dialogs;

                string player1Emotion = data.player1Emotion;
                string player2Emotion = data.player2Emotion;

                // cambia la emocion de ambos personajes.
                dialogManager.SetCharacterEmotion("Player 1", player1Emotion);
                dialogManager.SetCharacterEmotion("Player 2", player2Emotion);

                foreach (var dialog in nextDialogs)
                {
                    dialogQueue.Enqueue(dialog);
                }
            }
            else
            {
                dialogQueue.Enqueue(new DialogData("Bueno, sigamos...", "Player 2"));
            }

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