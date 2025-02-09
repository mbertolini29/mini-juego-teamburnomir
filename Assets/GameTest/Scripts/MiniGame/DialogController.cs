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
        [SerializeField] private GameObject character1;
        [SerializeField] private GameObject character2;
        [SerializeField] private GameObject dialog1;
        [SerializeField] private GameObject dialog2;
        [SerializeField] private TMP_Text textCharacter1;
        [SerializeField] private TMP_Text textCharacter2;

        private Queue<DialogData> dialogQueue = new Queue<DialogData>();
        private bool isConversationActive = false;

        private void Start()
        {
            StartConversation();
        }

        private void Update()
        {
            if(isConversationActive && (Input.GetKeyDown(KeyCode.Return) || 
                                        Input.GetMouseButtonDown(0)))
            {
                ShowNextDialog();
            }
        }

        private void StartConversation()
        {
            List<DialogData> dialogs = new List<DialogData>();
            
            dialogs.Add(new DialogData("Hola, como estas?", character1.name));
            dialogs.Add(new DialogData("Amigo, no sabés lo que me paso hoy.", character2.name));
            dialogs.Add(new DialogData("Uy, contamé", character1.name));
            dialogs.Add(new DialogData("Estaba caminando por av corriente y de repente..", character2.name));

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
                dialog1.SetActive(false);
                dialog2.SetActive(false);
                return;
            }

            var currentDialog = dialogQueue.Dequeue();
            bool isCharacter1Speking = currentDialog.Character == character1.name;

            // desactiva el panel de personaje que no esta hablando.
            dialog1.SetActive(isCharacter1Speking);
            dialog2.SetActive(!isCharacter1Speking);

            // muestra el dialogo correspondiente.
            if (isCharacter1Speking)
                textCharacter1.text = currentDialog.Commands[0].Context;
            else
                textCharacter2.text = currentDialog.Commands[0].Context;

            dialogManager.Show(currentDialog);
        }
    }
}
