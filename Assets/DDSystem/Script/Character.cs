using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Doublsb.Dialog
{
    //[RequireComponent(typeof(Image))]
    public class Character : MonoBehaviour
    {
        public Emotion Emotion;
        public AudioClip[] ChatSE;
        public AudioClip[] CallSE;

        private void Awake()
        {
            if(Emotion == null || Emotion._emotionObjects == null || Emotion._emotionObjects.Length == 0)
            {
                Debug.LogError($"El personaje {name} no tiene una referencia válida a Emotion.");
                return;
            }

            // Apagar todas las emociones al inicio
            foreach (var obj in Emotion._emotionObjects)
            {
                if (obj != null) obj.SetActive(false);
            }

            // Activar la emoción por defecto ("Normal")
            if (Emotion._emotionObjects.Length > 0 && Emotion._emotionObjects[0] != null)
            {
                Emotion._emotionObjects[0].SetActive(true);
            }
        }

        public void Emote(string emotion)
        {
            if (Emotion == null || Emotion._emotionObjects == null || Emotion._emotion == null)
            {
                Debug.LogError($"El personaje {name} no tiene emociones asignadas.");
                return;
            }

            int index = System.Array.IndexOf(Emotion._emotion, emotion);

            if (index < 0 || index >= Emotion._emotionObjects.Length)
            {
                Debug.LogWarning($"No se encontró la emoción '{emotion}' para el personaje {name}.");
                return;
            }

            // Apagar todas las caras antes de activar la nueva
            foreach (var obj in Emotion._emotionObjects)
            {
                if (obj != null) obj.SetActive(false);
            }

            // Activar la emoción seleccionada
            if (Emotion._emotionObjects[index] != null)
            {
                Emotion._emotionObjects[index].SetActive(true);
            }
        }

    }
}