using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;
using VNMaker.Progression;

[System.Serializable]
public class DialogueClass
{
    [SerializeField, Tooltip("Conditions needed to show this dialogue")] 
    private Conditions _PreConditions;

    [SerializeField] 
    private TextAsset _DialogueCSV;

    //List of monologues
    [SerializeField]
    private List<MonologueClass> _DialogueParts;

    //Conditions unlocked from this dialogue
    [SerializeField] 
    private Conditions _PostConditions;

    //Boolean needed to show multiple choices after the dialogue end
    [SerializeField] 
    private bool _HasChoices;
    [SerializeField] 
    private List<ChoiceClass> _DialogueChoices = new List<ChoiceClass>();

    //Boolean needed to show and set the name of the scene in which the dialogue will change at the end
    [SerializeField] 
    private bool _DoesSceneChangeAfter;

    [SerializeField] 
    private string _TargetSceneName;

    //this clip will be looped as the background music
    [SerializeField] 
    private AudioClip _DialogueMusicBackground;
    // [SerializeField] 
    // private float _LoopStartingPointInSeconds;

    public List<MonologueClass> DialogueParts => _DialogueParts;

    public bool HasChoices
    {
        get => _HasChoices;
        set => _HasChoices = value;
    }

    public List<ChoiceClass> DialogueChoices
    {
        get => _DialogueChoices;
        set => _DialogueChoices = value;
    }

    public Conditions PreConditions => _PreConditions;

    public Conditions PostConditions => _PostConditions;
    

    public TextAsset DialogueCSV
    {
        get => _DialogueCSV;
        set => _DialogueCSV = value;
    }

    public bool DoesSceneChangeAfter
    {
        get => _DoesSceneChangeAfter;
        set => _DoesSceneChangeAfter = value;
    }

    public string TargetSceneName
    {
        get => _TargetSceneName;
        set => _TargetSceneName = value;
    }

    public AudioClip DialogueMusicBackground
    {
        get => _DialogueMusicBackground;
        set => _DialogueMusicBackground = value;
    }

    // public float StartingLoopPoint
    // {
    //     get => _LoopStartingPointInSeconds;
    //     set => _LoopStartingPointInSeconds = value;
    // }
}