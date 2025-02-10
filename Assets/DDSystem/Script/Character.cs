using System;
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

        private Image _characterImage;

        private Transform parentTransform;

        private void Awake()
        {
            parentTransform = transform.parent;

            while(parentTransform != null)
            {
                _characterImage = parentTransform.GetComponent<Image>();
                if (_characterImage != null) break;
                parentTransform = parentTransform.parent;
            }
        }

        public void Emote(string emotion)
        {
            int index = Array.IndexOf(Emotion._emotion, emotion);

            if(index >= 0 && index < Emotion._sprite.Length)
            {
                _characterImage.sprite = Emotion._sprite[index];
                Debug.Log($"Personaje {name} cambio su emocion.");
            }
            else
            {
                Debug.LogWarning($"No se encontró la emocion {emotion} para el personaje {name}.");
            }
        }

    }
}