/*
The MIT License

Copyright (c) 2020 DoublSB
https://github.com/DoublSB/UnityDialogAsset

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Test;

namespace Doublsb.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        //================================================
        //Public Variable
        //================================================
        [Header("Panels")]
        public List<DialoguePanel> DialoguePanels;

        [Header("Type")]
        public bool isTyping = false;
        public bool isWaitingForNext = false;
        private string currentSentence = "";

        [Header("Mini Game")]
        public GameObject MiniGamePanel;
        public bool MiniGameIsFinished;
        public event Action<MemeQuality> OnMemeSelected;

        [Header("Audio Objects")]
        public AudioSource SEAudio;
        public AudioSource CallAudio;

        [Header("Preference")]
        public float Delay = 0.1f;

        [Header("Selector")]
        public GameObject Selector;
        public GameObject SelectorItem;
        public Text SelectorItemText;

        [HideInInspector]
        public State state;

        [HideInInspector]
        public string Result;

        //================================================
        //Private Method
        //================================================

        private Dictionary<string, DialoguePanel> _panelsDict = new Dictionary<string, DialoguePanel>();

        private DialogData _current_Data;
        private DialoguePanel _current_Panel;
        
        private Character _current_Character;

        private float _currentDelay;
        private float _lastDelay;
        private Coroutine _textingRoutine;
        private Coroutine _printingRoutine;

        private void Awake()
        {
            foreach (var dp in DialoguePanels)
            {
                if (!_panelsDict.ContainsKey(dp.CharacterID))
                    _panelsDict.Add(dp.CharacterID, dp);
                else
                    Debug.LogError($"Ya existe un panel con el ID: { dp.CharacterID}");
            }
        }

        //================================================
        //Public Method
        //================================================
        #region Show & Hide
        public void Show(DialogData data)
        {
            _current_Data = data;

            // buscas el panel correspondiente.
            if(!_panelsDict.TryGetValue(data.CharacterID, out _current_Panel))
            {
                Debug.LogError($"No se encontro el panel con ID: { data.CharacterID}");
                return;
            }

            _find_character(data.CharacterID, _current_Panel);

            // Activa el panel actual y oculta los demas. 
            foreach (var panel in _panelsDict.Values)
            {
                if (panel == _current_Panel)
                    panel.ShowPanel();
                else
                    panel.HidePanel();
            }

            // inicializar el texto en el panel actual
            _current_Panel.PrinterText.text = "";
            isTyping = true;
            isWaitingForNext = false;

            _textingRoutine = StartCoroutine(Activate());
        }

        public void Show(List<DialogData> Data)
        {
            StartCoroutine(Activate_List(Data));
        }

        public void Click_Window()
        {
            switch (state)
            {
                case State.Active:
                    StartCoroutine(_skip()); break;

                case State.Wait:
                    if(_current_Data.SelectList.Count <= 0) Hide(); break;
            }
        }

        public void Hide()
        {
            if(_textingRoutine != null)
                StopCoroutine(_textingRoutine);

            if(_printingRoutine != null)
                StopCoroutine(_printingRoutine);

            // ocultar todos los paneles.
            foreach (var panel in _panelsDict.Values)
            {
                panel.HidePanel();
            }

            state = State.Deactivate;

            if (_current_Data.Callback != null)
            {
                _current_Data.Callback.Invoke();
                _current_Data.Callback = null;
            }
        }
        #endregion

        #region Selector

        public void Select(int index)
        {
            Result = _current_Data.SelectList.GetByIndex(index).Key;
            Hide();
        }

        #endregion

        #region Sound

        public void Play_ChatSE()
        {
            if (_current_Character != null)
            {
                SEAudio.clip = _current_Character.ChatSE[UnityEngine.Random.Range(0, _current_Character.ChatSE.Length)];
                SEAudio.Play();
            }
        }

        public void Play_CallSE(string SEname)
        {
            if (_current_Character != null)
            {
                var FindSE
                    = Array.Find(_current_Character.CallSE, (SE) => SE.name == SEname);

                CallAudio.clip = FindSE;
                CallAudio.Play();
            }
        }

        #endregion

        #region Speed

        public void Set_Speed(string speed)
        {
            switch (speed)
            {
                case "up":
                    _currentDelay -= 0.25f;
                    if (_currentDelay <= 0) _currentDelay = 0.001f;
                    break;

                case "down":
                    _currentDelay += 0.25f;
                    break;

                case "init":
                    _currentDelay = Delay;
                    break;

                default:
                    _currentDelay = float.Parse(speed);
                    break;
            }

            _lastDelay = _currentDelay;
        }

        #endregion

        //================================================
        //Private Method
        //================================================

        private void _find_character(string name, DialoguePanel panel)
        {
            if (!string.IsNullOrEmpty(name))
            {
                GameObject characterObject = GameObject.Find(name);

                if(characterObject != null)
                {
                    _current_Character = characterObject.GetComponent<Character>();
                    
                    if (_current_Character == null)
                    {
                        Debug.LogWarning($"El gameobject {name} fue encontrado, pero no tiene un componente Character.");
                    }
                }
                else
                {
                    Debug.LogWarning($"No se encontró un gameobject con el nombre {name} en la escena.");
                }
                
                //Transform child = panel.CharactersContainer.transform.Find(name);
                //if (child != null)
                //    _current_Character = child.GetComponent<Character>();
                //else
                //    Debug.LogWarning($"No se encontró el personaje {name} en el panel {panel.CharacterID}");
            }
        }

        private void _initialize()
        {
            _currentDelay = Delay;
            _lastDelay = 0.1f;

            _current_Data.PrintText = "";
            _current_Panel.PrinterText.text = "";
        }

        private void _init_selector()
        {
            _clear_selector();

            if (_current_Data.SelectList.Count > 0)
            {
                Selector.SetActive(true);

                for (int i = 0; i < _current_Data.SelectList.Count; i++)
                {
                    _add_selectorItem(i);
                }
            }
                
            else Selector.SetActive(false);
        }

        private void _clear_selector()
        {
            // se asume que el primer hijo es la plantilla y los demas se generan.
            for (int i = 1; i < Selector.transform.childCount; i++)
            {
                Destroy(Selector.transform.GetChild(i).gameObject);
            }
        }

        private void _add_selectorItem(int index)
        {
            SelectorItemText.text = _current_Data.SelectList.GetByIndex(index).Value;

            var NewItem = Instantiate(SelectorItem, Selector.transform);
            NewItem.GetComponent<Button>().onClick.AddListener(() => Select(index));
            NewItem.SetActive(true);
        }

        #region Show Text

        private IEnumerator Activate_List(List<DialogData> DataList)
        {
            state = State.Active;

            foreach (var data in DataList)
            {
                Show(data);
                _init_selector();

                while (state != State.Deactivate) 
                    yield return null; 
            }
        }

        private IEnumerator Activate()
        {
            _initialize();

            state = State.Active;

            foreach (var item in _current_Data.Commands)
            {
                switch (item.Command)
                {
                    case Command.print:
                        yield return _printingRoutine = StartCoroutine(_print(item.Context));
                        break;

                    case Command.color:
                        _current_Data.Format.Color = item.Context;
                        break;

                    case Command.emote:
                        _emote(item.Context);
                        break;

                    case Command.size:
                        _current_Data.Format.Resize(item.Context);
                        break;

                    case Command.sound:
                        Play_CallSE(item.Context);
                        break;

                    case Command.speed:
                        Set_Speed(item.Context);
                        break;

                    case Command.click:
                        yield return _waitInput();
                        break;

                    case Command.close:
                        Hide();
                        yield break;

                    case Command.wait:
                        yield return new WaitForSeconds(float.Parse(item.Context));
                        break;

                }
            }

            state = State.Wait;

            //ejecutar el callback cuando termina el dialogo.
            _current_Data.Callback?.Invoke();
        }

        private IEnumerator _waitInput()
        {
            while (!Input.GetMouseButtonDown(0)) yield return null;
            _currentDelay = _lastDelay;
        }

        private IEnumerator _print(string text)
        {
            currentSentence = text;

            _current_Data.PrintText = "";
            _current_Panel.PrinterText.text = "";

            foreach (char letter in text.ToCharArray())
            {
                if (!isTyping) yield break; // Detiene la animación si se interrumpe

                _current_Data.PrintText += letter;
                _current_Panel.PrinterText.text = _current_Data.PrintText;

                if (letter != ' ') Play_ChatSE();
                yield return new WaitForSeconds(_currentDelay);
            }

            isTyping = false;
            isWaitingForNext = true;
        }

        public void SkipTyping()
        {
            if (isTyping)
            {
                StopCoroutine(_printingRoutine);  // Detiene la animación de texto
                _current_Panel.PrinterText.text = currentSentence; // Muestra la frase completa
                isTyping = false;
                isWaitingForNext = true;
            }
        }

        public void _emote(string emotion)
        {
            //if(_current_Character != null && _current_Character.Emotion.Data.ContainsKey(emotion))
            //    _current_Character.GetComponent<Image>().sprite = _current_Character.Emotion.Data[emotion];
        }

        private IEnumerator _skip()
        {
            if (_current_Data.isSkippable)
            {
                _currentDelay = 0;
                while (state != State.Wait) yield return null;
                _currentDelay = Delay;
            }
        }

        #endregion

        #region Mini Game

        //este funcionaba individualmente..
        public void MemeSelected(MemeQuality quality)
        {
            if(_current_Character == null)
            {
                Debug.LogError("No hay un personaje activo para cambiar la emocion.");
                return;
            }

            switch (quality)
            {
                case MemeQuality.Normal:
                    _current_Character.Emote("Normal");
                    break;
                case MemeQuality.Happy:

                    _current_Character.Emote("Happy");
                    break;
                case MemeQuality.Sad:
                    _current_Character.Emote("Sad");
                    break;                
            }

            OnMemeSelected?.Invoke(quality);
        }

        //este cambia ambos personajes.
        public void SetCharacterEmotion(string characterID, string emotion)
        {
            if(_panelsDict.TryGetValue(characterID, out DialoguePanel panel))
            {
                Character character = panel.CharactersContainer.GetComponentInChildren<Character>();

                if(character != null)
                {
                    character.Emote(emotion);
                }
                else
                {
                    Debug.LogWarning($"No se encontró el personaje {characterID} en el díalogo.");
                }
            }
        }

        #endregion
    }
}