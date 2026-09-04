using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        // Serialized fields for Unity Inspector
        [SerializeField] protected List<DialogueSO> _Dialogues; // List of dialogue scriptable objects

        public List<DialogueSO> Dialogues
        {
            get => _Dialogues;
            set => _Dialogues = value;
        }

        [SerializeField] private DialogueSO _DefaultDialogue; // Default dialogue if no specific dialogues are available

        public DialogueSO DefaultDialogue
        {
            get => _DefaultDialogue;
            set => _DefaultDialogue = value;
        }

       // private EventTrigger _eventTrigger;
        private EventTrigger.Entry _entry = new();

        private void Start()
        {
            if (TryGetComponent<EventTrigger>(out var eventTrigger))
            {
                _entry.eventID = EventTriggerType.PointerClick;
                _entry.callback.AddListener((_) => { StartDialogue(); });
                eventTrigger.triggers.Add(_entry);
            }
        }

        /// <summary>
        /// Initiates the dialogue sequence by collecting all dialogues from the list and passing them 
        /// to the DialogueElaborator along with the default dialogue.
        /// </summary>
        public void StartDialogue()
        {
            // Create a list to store dialogues to be processed
            List<DialogueClass> dialogueList = new List<DialogueClass>();

            // Add dialogues from the serialized list to the local list
            for (int i = 0; i < _Dialogues.Count; i++)
            {
                dialogueList.Add(_Dialogues[i].Dialogue);
            }

            DialogueClass defaultDialogue = null;

            if (dialogueList.Count == 0 && _DefaultDialogue.Dialogue != null)
            {
                defaultDialogue = _DefaultDialogue.Dialogue;
                dialogueList.Add(defaultDialogue);
            }

            // Pass the dialogues to the DialogueElaborator for processing
            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.START_DIALOGUE_ELAB, dialogueList, defaultDialogue);
        }
    }
}