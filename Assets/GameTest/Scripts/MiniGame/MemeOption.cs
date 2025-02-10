using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Test
{
    public enum MemeQuality
    {
        Normal,
        Happy,
        Sad
    }

    public class MemeOption : MonoBehaviour
    {
        public string MemeName;
        public MemeQuality Quality;
        public Image MemeImage;

        public Action<MemeQuality> OnMemeSelected; 

        public void SelectMeme()
        {
            OnMemeSelected?.Invoke(Quality);
        }
    }
}
