using System.Collections;
using DialogueSystem;
using UnityEngine;

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