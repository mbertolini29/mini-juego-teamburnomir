using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Test
{
    public class CinematicManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text cinematicText;
        [SerializeField] private GameObject cinematicPanel;
        [SerializeField] private Image dialogPanel;
        
        [SerializeField] private float speedText = 0.05f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float timeBeforeNextLine = 0.5f;
        [SerializeField] private float timeChangeScene = 0.5f;

        private Queue<string> cinematicLines = new Queue<string>();

        private bool isTyping = false;
        private bool isWaitingForNext = false;
        private string currentSentence = "";

        private Coroutine typingCoroutine;

        private void Start()
        {
            // Agregar lineas de la cinemática.
            cinematicLines.Enqueue("Argentina, un país con una cultura en la que...");
            cinematicLines.Enqueue("Si tuviste un día en el que sentiste que todo salió mal...");
            cinematicLines.Enqueue("O si recibiste una buena noticia y explotás de emoción...");
            cinematicLines.Enqueue("Siempre hay un amigo dispuesto a escucharte.");
            cinematicLines.Enqueue("Una juntada, la pizza y la Play hacen que la vida se sienta más liviana.");

            cinematicPanel.SetActive(true);
            dialogPanel.color = new Color(0, 0, 0, 0); // Inicia invisible
            StartCoroutine(FadeIn());

            StartCoroutine(WaitBeforeNextLine());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                if(isTyping)
                {
                    StopCoroutine(typingCoroutine);
                    cinematicText.text = currentSentence;
                    isTyping = false;
                    isWaitingForNext = true;
                }
                else if(isWaitingForNext)
                {
                    isWaitingForNext = false;
                    StartCoroutine(WaitBeforeNextLine());
                }
            }
        }

        private IEnumerator WaitBeforeNextLine()
        {
            yield return new WaitForSeconds(timeBeforeNextLine);
            ShowNextLine();
        }

        public void ShowNextLine()
        {
            if(cinematicLines.Count == 0)
            {
                StartCoroutine(EndCinematic());
                return;
            }

            currentSentence = cinematicLines.Dequeue();
            typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
        }

        private IEnumerator TypeSentence(string sentence)
        {
            isTyping = true;
            cinematicText.text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                cinematicText.text += letter;
                yield return new WaitForSeconds(speedText);
            }

            isTyping = false;
            isWaitingForNext = true;
        }

        private IEnumerator EndCinematic()
        {
            yield return new WaitForSeconds(timeChangeScene);
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                dialogPanel.color = new Color(0, 0, 0, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            dialogPanel.color = new Color(0, 0, 0, 1);
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                dialogPanel.color = new Color(0, 0, 0, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            dialogPanel.color = new Color(0, 0, 0, 0);
            cinematicPanel.SetActive(false);
            LoadGameplayScene();
        }

        public void LoadGameplayScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
