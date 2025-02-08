using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class Character : MonoBehaviour
    {
        public string characterName;
        public EmotionState currentEmotion;

        public void ChangeEmotion(EmotionState newEmotion)
        {
            currentEmotion = newEmotion;

            //cambiar la animacion del dialogo.
        }
    }

    public enum EmotionState
    {
        Neutral,
        Sad,
        Happy,
        Angry
    }
}
