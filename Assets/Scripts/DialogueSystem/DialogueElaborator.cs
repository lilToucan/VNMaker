using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VNMaker.EventBuss;
using VNMaker.Progression;
using VNMaker.Singletons;

namespace DialogueSystem
{
    public class DialogueElaborator : MonoBehaviour
    {
        private bool _isDialogueActive = false;

        private DialogueClass _dialogue;
        private string _currentText;
        private SerializedDictionary<Sprite, List<AnimationClip>> _currentImages = new();
        private int _currentMonologueIndex;
        private int _currentSentenceIndex;

        private void Start()
        {
            GameManager.Instance.DialogueEvents.Register(DialogueEventList.START_DIALOGUE_ELAB, StartDialogue); // trigger found in DialogueTrigger class
        }

        /// <summary>
        /// Get's the dialogue from the list of dialogues 
        /// </summary>
        /// <param name="dialogueList"> List[DialogueClass]</param>
        /// <param name="defaultDialogue">DialogueClass</param>
        public void StartDialogue(object[] param)
        {
            if (_isDialogueActive)
                return;

            List<DialogueClass> dialogueList = (List<DialogueClass>)param[0];
            DialogueClass defaultDialogue = null;
            if (param.Length > 1)
            {
                defaultDialogue = (DialogueClass)param[1];
            }


            if (dialogueList != null && dialogueList.Count > 0)
            {
                //checking which dialogue from the list is the one who respects the preconditions
                for (int i = 0; i < dialogueList.Count; i++)
                {
                    if (ConditionsUtils.CheckConditions(dialogueList[i].PreConditions))
                    {
                        _dialogue = dialogueList[i];
                        break;
                    }
                }
            }

            if (_dialogue == null)
            {
                if (defaultDialogue == null)
                    return;
                _dialogue = defaultDialogue;
            }


            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.START_DIALOGUE); // found in the DialogueUiClass

            if (_dialogue.DialogueMusicBackground != null)
            {
                GameManager.Instance.DialogueSoundManager.PlayOnLoop(_dialogue.DialogueMusicBackground, _dialogue.StartingLoopPoint);
            }

            _isDialogueActive = true;
            StartMonologue();
        }

        /// <summary>
        /// Start the monologue based on CurrentMonologueIndex and show the first sentence
        /// </summary>
        public void StartMonologue()
        {
            if (_dialogue != null)
            {
                GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.CHANGE_NAME, _dialogue.DialogueParts[_currentMonologueIndex].SName);
                ClearCurrent();

                foreach (SentenceClass sentence in _dialogue.DialogueParts[_currentMonologueIndex].Sentences)
                {
                    SetCurrent(sentence.Sentence, sentence.SpriteAnimMap);
                    if (sentence.SFXAudio != null)
                    {
                        GameManager.Instance.DialogueSoundManager.PlayOneShotSound(sentence.SFXAudio);
                    }
                }

                NextSentence(); // in this case first sentence 
            }
        }

        /// <summary>
        /// Show the first sentence available in the list of CurrentText
        /// </summary>
        public void NextSentence()
        {
            if (_currentText == null || _dialogue == null || _dialogue.DialogueParts == null || _dialogue.DialogueParts.Count <= 0)
                return;

            //If i reached the last sentence of the current monologue
            if (_currentSentenceIndex == _dialogue.DialogueParts[_currentMonologueIndex].Sentences.Count)
            {
                // if the current monologue wasn't the last,  start the next monologue
                if (_currentMonologueIndex < _dialogue.DialogueParts.Count - 1)
                {
                    _currentMonologueIndex++;
                    StartMonologue();
                }
                else
                {
                    if (_dialogue.HasChoices)
                    {
                        EndDialogue();
                        GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.START_CHOICE, _dialogue.DialogueChoices);
                    }
                    else
                    {
                        EndDialogue();
                        GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.CHECK_POST_INTERACTION);
                    }
                }

                return;
            }

            TypeSentence();
            _currentSentenceIndex++;
        }

        /// <summary>
        /// Type letter by letter the entire senteces passed by param in UI
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        private void TypeSentence()
        {
            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.CHANGE_SENTENCE, "");
            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.CHANGE_SENTENCE, _currentText);
            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.CHANGE_IMAGE, _currentImages);
        }

        /// <summary>
        /// Stop dialogue, reset all to default values and hide text box in UI
        /// </summary>
        private void EndDialogue()
        {
            ConditionsUtils.ApplyCondition(_dialogue.PostConditions);

            _currentMonologueIndex = 0;
            _currentSentenceIndex = 0;

            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.END_DIALOGUE, _dialogue);
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION);

            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.END_DIALOGUE);

            _isDialogueActive = false;
            if (_dialogue != null && _dialogue.HasSceneChange && !string.IsNullOrEmpty(_dialogue.TargetSceneName))
            {
                SceneManager.LoadScene(_dialogue.TargetSceneName);
            }

            _dialogue = null;
        }

        private void ClearCurrent()
        {
            _currentSentenceIndex = 0;
            _currentText = "";
            _currentImages.Clear();
        }

        private void SetCurrent(string sentence, SerializedDictionary<Sprite, List<AnimationClip>> image)
        {
            _currentText = sentence;
            _currentImages = image;
        }
    }
}