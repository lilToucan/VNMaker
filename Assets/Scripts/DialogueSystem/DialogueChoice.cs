using UnityEngine;
using UnityEngine.UI;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace DialogueSystem
{
    public class DialogueChoice : MonoBehaviour
    {
        private Button _choiceBtn;
        private DialogueTrigger _dialogueTrigger;
        
        public Button Button => _choiceBtn;
        public DialogueTrigger DialogueTrigger => _dialogueTrigger;

        private void Start()
        {
            _choiceBtn = gameObject.GetComponent<Button>();
            _dialogueTrigger = gameObject.GetComponent<DialogueTrigger>();

            _choiceBtn.onClick.AddListener(_dialogueTrigger.StartDialogue);
            _choiceBtn.onClick.AddListener(HideChoice);
        }

        private void HideChoice()
        {
            GameManager.Instance.DialogueEvents.TriggerEvent(DialogueEventList.HIDE_CHOICE);
        }
    }
}