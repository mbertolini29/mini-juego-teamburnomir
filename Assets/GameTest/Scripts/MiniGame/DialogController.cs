using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using TMPro;
using System;

namespace Test
{
    public class DialogController : MonoBehaviour
    {
        [SerializeField] private DialogManager dialogManager;

        [Header("Mini Game 1")]
        [SerializeField] private GameObject miniGamePanel;
        [SerializeField] private List<MemeOption> miniGameMemes;
        
        [Header("Mini Game 2")]
        [SerializeField] private GameObject miniGamePanel2;
        [SerializeField] private List<MemeOption> miniGameMemes2;
        
        [Header("Mini Game 3")]
        [SerializeField] private GameObject miniGamePanel3;
        [SerializeField] private List<MemeOption> miniGameMemes3;

        private Queue<DialogData> dialogQueue = new Queue<DialogData>();
        private bool isMiniGameActive = false;
        private int miniGameCounter = 0;

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
            if (!isMiniGameActive && Time.timeScale > 0)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    if (dialogManager.isTyping)
                    {
                        dialogManager.SkipTyping(); // Ahora usamos el nuevo método para saltar la escritura
                    }
                    else if (dialogManager.isWaitingForNext)
                    {
                        dialogManager.isWaitingForNext = false;
                        ShowNextDialog();
                    }
                }
            }            
        }

        private void StartConversation()
        {
            List<DialogData> dialogs = new List<DialogData>();

            dialogs.Add(new DialogData("Amigo, no sabés el garrón que me comí hoy en el súper.", "Player 1"));
            dialogs.Add(new DialogData("A ver, contá", "Player 2"));
            dialogs.Add(new DialogData("Entró tranqui, con la idea de comprar dos boludeces...", "Player 1"));
            dialogs.Add(new DialogData("Pero viste cómo es... terminé con el carrito hasta el borde.", "Player 1"));
            dialogs.Add(new DialogData("Primero porque me acordé que no tenía yerba...", "Player 1"));
            dialogs.Add(new DialogData("Después, vi una promo de fideos, queso rallado, fiambre...", "Player 1"));
            dialogs.Add(new DialogData("Y cuando me quise dar cuenta parecía que estaba estoqueándome para el fin del mundo.", "Player 1"));
            dialogs.Add(new DialogData("Un clásico.", "Player 2"));
            dialogs.Add(new DialogData("Bueno, llego a la caja y adelante mío había un señor con UNA sola cosa: un paquete de galletitas... ", "Player 1"));
            dialogs.Add(new DialogData("Y yo pensé! Uh, qué suerte, esto va a ser rápido. ERROR. ", "Player 1", () => MiniGame()));

            dialogs.Add(new DialogData("Nooo, ¿y que paso?", "Player 2"));
            dialogs.Add(new DialogData("El tipo le pasa las galletitas a la cajera, la mina le dice el precio...", "Player 1"));
            dialogs.Add(new DialogData("Y el chabón se queda quieto, mirándola fijo: no, no, no. Esto estaba en oferta.", "Player 1"));
            dialogs.Add(new DialogData("Y ahí arrancó el quilombo.", "Player 1"));
            dialogs.Add(new DialogData("Ya me la veo venir.", "Player 2"));
            dialogs.Add(new DialogData("La cajera le dice el precio, pero el tipo insiste con que el cartel decía otra cosa...", "Player 1"));
            dialogs.Add(new DialogData("Llaman al supervisor, mandan a una empleada a chequear...", "Player 1"));
            dialogs.Add(new DialogData("Y yo ahí, viendo cómo la vida me castigaba por no haber ido a la caja rápida ", "Player 1", () => MiniGame()));

            dialogs.Add(new DialogData("JAJAJA, es que el destino te odia, amigo.", "Player 2"));
            dialogs.Add(new DialogData("Mal. Bueno, después de como cinco minutos, vuelve la chica y confirma que sí, el cartel decía otro precio...", "Player 1"));
            dialogs.Add(new DialogData("Pero la cajera le explica que la promo era si llevabas dos... ", "Player 1"));
            dialogs.Add(new DialogData("Y el tipo, en vez de pagar y rajar, ¿qué hace? ¡SE VA A BUSCAR OTRA! ", "Player 1"));
            dialogs.Add(new DialogData("Pero como no la encuentra, se pone a buscar a la empleada para que lo ayude...", "Player 1"));
            dialogs.Add(new DialogData("Y yo mientras tanto ahí, con cara de estatua, con la cerveza que ya estaba re caliente.", "Player 1"));
            dialogs.Add(new DialogData("JAJAJAJAJA", "Player 2"));
            dialogs.Add(new DialogData("Al final, vuelve con las dos galletitas, paga y se va re contento, como si hubiera ganado un juicio.", "Player 1", () => MiniGame()));

            dialogs.Add(new DialogData("Y vos al fin pudiste pagar.", "Player 2"));
            dialogs.Add(new DialogData("Sí, pero cuando salgo del súper, confiado, respiro hondo y pienso:", "Player 1"));
            dialogs.Add(new DialogData("Listo, lo logré! Doy un paso… ¡y PISO UNA BALDOSA FLOJA!", "Player 1"));
            dialogs.Add(new DialogData("NOOOO", "Player 2"));
            dialogs.Add(new DialogData("¡Sí! Agua turbia volando para todos lados, yo empapado y las bolsas a punto de reventar.", "Player 1"));
            dialogs.Add(new DialogData("Y para coronarla, pasa una doña, me mira y me dice: Ay, nene, eso es señal de buena suerte.", "Player 1"));
            dialogs.Add(new DialogData("JAJAJAJAJA", "Player 2"));
            dialogs.Add(new DialogData("Si esto es buena suerte, prefiero que me vaya mal.", "Player 1"));


            foreach (var dialog in dialogs)
            {
                dialogQueue.Enqueue(dialog);
            }

            ShowNextDialog();
        }

        private void ShowNextDialog()
        {
            if (dialogQueue.Count == 0)
            {
                //dialogManager.Hide();
                Debug.Log("Todos los diálogos terminados. Cerrando escena.");
                StartCoroutine(EndScene());
                return;
            }

            if (isMiniGameActive)
            {
                Debug.Log("Esperando que termine el Mini-Game antes de continuar.");
                return; // IMPORTANTE: No sigue avanzando hasta que termine el mini-juego
            }

            var currentDialog = dialogQueue.Dequeue();

            if (currentDialog.Callback != null)
            {
                Debug.Log($"Mini-Game detectado en el diálogo: {currentDialog.PrintText}");
                dialogManager.Show(currentDialog);

                isMiniGameActive = true; // Activa el mini-juego antes de mostrarlo

                //currentDialog.Callback.Invoke();
                Action miniGameAction = () => currentDialog.Callback.Invoke(); 
                StartCoroutine(WaitThenStartMiniGame(miniGameAction));

            }
            else
            {
                dialogManager.Show(currentDialog);
            }

        }
        private IEnumerator WaitThenStartMiniGame(Action miniGameCallback)
        {
            yield return new WaitUntil(() => !dialogManager.isTyping); 
            yield return new WaitForSeconds(2.0f); // sin el istyping --> 12 seg.
            miniGameCallback?.Invoke();
        }

        private void MiniGame()
        {
            Debug.Log($"🟢 MiniGame() llamado - Estado: {dialogManager.state}, MiniGameCounter: {miniGameCounter}");

            if (dialogManager.state == State.Deactivate)
            {
                Debug.LogWarning("No se puede iniciar el mini-juego porque el estado está desactivado.");
                dialogManager.state = State.Active; // Forzar estado activo
            }

            StartCoroutine(StartMiniGameWithDelay());
        }

        private IEnumerator StartMiniGameWithDelay()
        {
            Debug.Log($"StartMiniGameWithDelay iniciado - MiniGameCounter: {miniGameCounter}");

            yield return new WaitForSeconds(0.5f);

            dialogManager.Hide();

            isMiniGameActive = true;

            switch (miniGameCounter)
            {
                case 0:
                    Debug.Log("Activando Mini-Game 1");
                    SetupMiniGame(miniGameMemes); // configurar la imagen de los memes individualmente.  
                    miniGamePanel.SetActive(true);
                    break;
                case 1:
                    Debug.Log("Activando Mini-Game 2");
                    SetupMiniGame(miniGameMemes2); // configurar la imagen de los memes individualmente.  
                    miniGamePanel2.SetActive(true);
                    break;
                case 2:
                    Debug.Log("Activando Mini-Game 3");
                    SetupMiniGame(miniGameMemes3); // configurar la imagen de los memes individualmente.  
                    miniGamePanel3.SetActive(true);
                    break;
                default:
                    Debug.LogError("MiniGameCounter fuera de rango");
                    break;
            }

        }

        public void OnMiniGameFinished(MemeQuality quality)
        {
            Debug.Log($"Finalizando MiniGame {miniGameCounter} con calidad: {quality}");

            miniGamePanel.SetActive(false);
            miniGamePanel2.SetActive(false);
            miniGamePanel3.SetActive(false);

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
            //else
            //{
            //    dialogQueue.Enqueue(new DialogData("Bueno, sigamos...", "Player 2"));
            //}

            miniGameCounter++;

            if(miniGameCounter >= 3)
            {
                Debug.Log("Todos los mini-juegos completados. Finalizando escena...");
                StartCoroutine(EndScene());
                return;
            }

            Debug.Log($"MiniGameCounter ahora es: {miniGameCounter}");

            ShowNextDialog();
        }

        private IEnumerator EndScene()
        {
            Debug.Log("Volviendo al menú principal...");
            
            yield return new WaitForSeconds(1.0f);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private void SetupMiniGame(List<MemeOption> miniGameMemes)
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