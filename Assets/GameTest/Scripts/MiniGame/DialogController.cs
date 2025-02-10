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

        private Queue<DialogData> dialogQueue = new Queue<DialogData>();
        private bool isConversationActive = false;

        private void Start()
        {
            StartConversation();
        }

        private void Update()
        {
            //if (!dialogManager.isTyping)

            if (isConversationActive && (Input.GetKeyDown(KeyCode.Return) ||
                                         Input.GetMouseButtonDown(0)))
            {
                if (dialogManager.isTyping)
                {
                    dialogManager.isTyping = false;
                }
                else
                {
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
            
            Debug.Log("Iniciando MiniGame...");

            // ocultas los dialogos
            dialogManager.Hide();

            // activa el panel de mini juego.
            miniGamePanel.SetActive(true);

            // supongamos que tenes 5 segundo para responder... 
            // aunque para mi sin tiempo.
            StartCoroutine(SimulateMiniGame());
        }

        private IEnumerator SimulateMiniGame()
        {
            Debug.Log("MiniGame simulado: esperando 5 segundos...");

            //logica del juego..

            yield return new WaitForSeconds(2.0f);

            OnMiniGameFinished();
        }

        public void OnMiniGameFinished()
        {
            Debug.Log("MiniGame finalizado. Reanudando conversación...");

            miniGamePanel.SetActive(false);

            dialogManager.state = State.Active;
            ShowNextDialog();
        }

    }
}