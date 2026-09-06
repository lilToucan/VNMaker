using System.Collections;
using DialogueSystem;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// This class automatically calls the StartDialogue function of any dialogue trigger component at the start
    /// </summary>
    [RequireComponent(typeof(DialogueTrigger))]
    public class AutomaticOnSceneChangeDialogue : MonoBehaviour
    {
        private DialogueTrigger _xDialogueTrigger;


        void Start()
        {
            _xDialogueTrigger = GetComponent<DialogueTrigger>();
            StartCoroutine(StartDialogue());
        }

        IEnumerator StartDialogue()
        {
            yield return new WaitForSeconds(0.5f);
            _xDialogueTrigger.StartDialogue();
        }
    }
}