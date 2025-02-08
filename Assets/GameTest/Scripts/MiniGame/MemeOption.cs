using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class MemeOption 
    {
        [SerializeField] private string memeText;
        [SerializeField] private Sprite memeSprite;
        [SerializeField] private int graceLevel; // 0 malo, 1 = normal, 2 = gracioso. //grace = gracia.

        public string MemeText => memeText;
        public Sprite MemeSprite => memeSprite;
        public int GraceLevel => graceLevel;
    }
}
