using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Test
{
    public class DialoguePanel : MonoBehaviour
    {
        [Header("Identificador del panel")]
        public string CharacterID = "Player 1";

        [Header("Elementos de UI")]
        public GameObject PanelObject; //Panel padre que se activa o desactiva.
        public TMP_Text PrinterText; // donde se muestra el dialogo.
        public GameObject CharactersContainer; //donde se ubican los personajes. 
        
        public void ShowPanel()
        {
            if (PanelObject != null) PanelObject.SetActive(true);
        }

        public void HidePanel()
        {
            if (PanelObject != null) PanelObject.SetActive(false);
        }
    }
}
