using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class ChoiceClass
    {
        [SerializeField] private string _Label;
        [SerializeField] private DialogueSO _DialogueSO;

        public string ChoiceLabel { get => _Label; set => _Label = value; }
        public DialogueSO ChoiceDialogueSO { get => _DialogueSO; set => _DialogueSO = value; }
    }
}