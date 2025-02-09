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
using TMPro;

namespace Doublsb.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        //================================================
        //Public Variable
        //================================================
        [Header("Game Objects")]
        public GameObject Characters;

        [Header("Audio Objects")]
        public AudioSource SEAudio;
        public AudioSource CallAudio;

        [Header("Preference")]
        public float Delay = 0.1f;

        [Header("Selector")]
        public GameObject Selector;
        public GameObject SelectorItem;
        public TMP_Text SelectorItemText;

        [HideInInspector]
        public State state;

        [HideInInspector]
        public string Result;

        //================================================
        //Private Method
        //================================================
        private Dictionary<string, Character> characterDict = new Dictionary<string, Character>();

        private DialogData _current_Data;

        private float _currentDelay;
        private float _lastDelay;
        private Coroutine _textingRoutine;
        private Coroutine _printingRoutine;

        private void Start()
        {
            LoadCharacters();        
        }

        private void LoadCharacters()
        {
            characterDict.Clear();
            foreach (Transform child in Characters.transform)
            {
                Character character = child.GetComponentInChildren<Character>();
                if (character != null)
                {
                    characterDict[character.name] = character;
                }
            }
        }

        //================================================
        //Public Method
        //================================================
        #region Show & Hide
        public void Show(DialogData Data)
        {
            _current_Data = Data;

            if (!characterDict.ContainsKey(Data.Character))
            {
                Debug.LogError($"Error: No se encontró el personaje '{Data.Character}' en la escena.");
                return;
            }

            _textingRoutine = StartCoroutine(Activate(Data.Character));
        }

        public void Show(List<DialogData> DataList)
        {
            StartCoroutine(Activate_List(DataList));
        }

        public void Click_Window()
        {
            if (state == State.Active)
                StartCoroutine(_skip());
            else if (state == State.Wait && _current_Data.SelectList.Count <= 0)
                Hide();
        }

        public void Hide()
        {
            if(_textingRoutine != null) StopCoroutine(_textingRoutine);
            if(_printingRoutine != null) StopCoroutine(_printingRoutine);

            foreach (var character in characterDict.Values)
            {
                //character.HideDialog();
            }

            Selector.SetActive(false);
            state = State.Deactivate;

            _current_Data?.Callback?.Invoke();
            _current_Data.Callback = null;
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

        public void Play_ChatSE(string characterName)
        {
            if (!characterDict.ContainsKey(characterName)) return;
            Character character = characterDict[characterName];

            if (character.ChatSE.Length > 0)
            {
                SEAudio.clip = character.ChatSE[UnityEngine.Random.Range(0, character.ChatSE.Length)];
                SEAudio.Play();
            }
        }

        public void Play_CallSE(string characterName, string SEname)
        {
            if (!characterDict.ContainsKey(characterName)) return;
            Character character = characterDict[characterName];

            var FindSE = Array.Find(character.CallSE, (SE) => SE.name == SEname);
            if (FindSE != null)
            {
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

        private IEnumerator Activate(string characterName)
        {
            _initialize(characterName);
            //state = State.Active;

            foreach (var item in _current_Data.Commands)
            {
                if (item.Command == Command.print)
                {
                    yield return _printingRoutine = StartCoroutine(_print(item.Context, characterName));
                }
            }

            //state = State.Wait;
        }

        private void _initialize(string characterName)
        {
            _currentDelay = Delay;
            _lastDelay = 0.1f;

            if (!characterDict.ContainsKey(characterName)) return;

            //Character character = characterDict[characterName];
            //character.ShowDialog();
        }

        private IEnumerator _print(string text, string characterName)
        {
            if (!characterDict.ContainsKey(characterName)) yield break;

            Character character = characterDict[characterName];
            //character.SetDialogText(text);

            if (text.Length > 0)
            {
                Play_ChatSE(characterName);
            }

            yield return new WaitForSeconds(_currentDelay);
        }

        #region Show Text



        private IEnumerator Activate_List(List<DialogData> DataList)
        {
            state = State.Active;

            foreach (var Data in DataList)
            {
                Show(Data);
                while (state != State.Deactivate) { yield return null; }
            }
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

    }
}