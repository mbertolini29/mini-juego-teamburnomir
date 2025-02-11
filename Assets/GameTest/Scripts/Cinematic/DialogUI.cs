using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Test
{
    public class DialogUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup dialogCanvas;
        [SerializeField] private TMP_Text dialogText;
        [SerializeField] private float fadeDuration = 0.5f;

        public void ShowDialog(string text)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInDialog(text));
        }

        private IEnumerator FadeInDialog(string text)
        {
            dialogCanvas.alpha = 0;
            dialogCanvas.gameObject.SetActive(true);

            //fade in
            float t = 0;
            while(t < fadeDuration)
            {
                dialogCanvas.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }

            dialogCanvas.alpha = 1;

            dialogText.text = text;
        }

        public void HideDialog()
        {
            StartCoroutine(FadeOutDialog());
        }

        private IEnumerator FadeOutDialog()
        {
            float t = 0;
            while (t < fadeDuration)
            {
                dialogCanvas.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }

            dialogCanvas.alpha = 0;
            dialogCanvas.gameObject.SetActive(false);
        }
    }
}
