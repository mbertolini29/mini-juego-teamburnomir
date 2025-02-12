using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;  // Para acceder desde otros scripts.
        public AudioSource audioSource;
        public AudioClip buttonClickSound;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void PlayButtonClick()
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}
