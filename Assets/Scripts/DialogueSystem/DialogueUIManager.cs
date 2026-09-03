using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DialogueSystem;
using TMPro;
using UnityEngine.UI;
using VNMaker.EventBuss;
using VNMaker.Singletons;

public class DialogueUiManager : MonoBehaviour
{
    // Serialized fields for UI components
    [SerializeField] private GameObject _MenuBackground;
    [SerializeField] private TextMeshProUGUI _Sentence;
    [SerializeField] private Image _TextBox;
    [SerializeField] private GameObject _NameTextBox;
    [SerializeField] private TextMeshProUGUI _Name;
    [SerializeField] private GameObject _ListOfCharacters;
    private List<CharacterSlot> _characters = new();
    [SerializeField] private Button _ContinueBtn;
    [SerializeField] private GameObject _ChoicesBox;
    [SerializeField] private GameObject _ChoicesContent;
    [SerializeField] private GameObject _ChoicePrefab;

    // Initialization and event registration
    void Start()
    {
        // Registering functions to EventManager
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.START_DIALOGUE, ShowDialogueUI);
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.CHANGE_NAME, ChangeName);
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.CHANGE_SENTENCE, ChangeSentence);
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.CHANGE_IMAGE, ChangeImage);
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.END_DIALOGUE, HideDialogueUI);
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.START_CHOICE, ShowChoices);
        GameManager.Instance.DialogueEvents.Register(DialogueEventList.HIDE_CHOICE, HideChoices);

        HideDialogueUI(null);

        // Get all character slots in 
        for (int i = 0; i < _ListOfCharacters.transform.childCount; i++)
        {
            if (!_ListOfCharacters.transform.GetChild(i).gameObject.TryGetComponent<CharacterSlot>(out var character))
                continue;

            _characters.Add(character);
        }
    }

    /// <summary>
    /// Changes the dialogue sentence displayed in the UI.
    /// </summary>
    /// <param name="param">Array where the first element is the sentence (string).</param>
    public void ChangeSentence(object[] param)
    {
        _Sentence.text = (string)param[0];
    }

    /// <summary>
    /// Updates the character name displayed in the UI.
    /// </summary>
    /// <param name="param">Array where the first element is the name (string).</param>
    public void ChangeName(object[] param)
    {
        _Name.text = (string)param[0];
    }

    /// <summary>
    /// Updates character images shown in the UI based on a list of sprites.
    /// </summary>
    /// <param name="param">Array where the first element is a list of Sprite arrays.</param>
    public void ChangeImage(object[] param)
    {
        SerializedDictionary<Sprite, List<AnimationClip>> charactersInfo = (SerializedDictionary<Sprite, List<AnimationClip>>)param[0];

        int i = 0;
        foreach (KeyValuePair<Sprite, List<AnimationClip>> character in charactersInfo)
        {
            _characters[i].SpriteHolder.sprite = character.Key;
            _characters[i].SpriteHolder.gameObject.SetActive(true);
            _characters[i].PlayAnimations(character.Value);
            i++;
        }
    }

    /// <summary>
    /// Fills the choice box with options and their corresponding dialogues.
    /// </summary>
    /// <param name="param">Array containing dialogues and their labels.</param>
    private void FillChoiceBox(object[] param)
    {
        if (param != null & param.Length > 0)
        {
            List<ChoiceClass> dialogues = (List<ChoiceClass>)param[0];

            for (int i = 0; i < dialogues.Count; i++)
            {
                GameObject tmp = Instantiate(_ChoicePrefab, _ChoicesContent.transform);
                tmp.GetComponent<DialogueTrigger>().DefaultDialogue = dialogues[i].ChoiceDialogueSO;
                tmp.GetComponentInChildren<TMP_Text>().text = dialogues[i].ChoiceLabel;
            }
        }
    }

    /// <summary>
    /// Hides all dialogue-related UI elements.
    /// </summary>
    /// <param name="param">dialogue</param>
    private void HideDialogueUI(object[] param)
    {
        _ContinueBtn.gameObject.SetActive(false);
        _TextBox.gameObject.SetActive(false);
        _MenuBackground.gameObject.SetActive(false);
        _Sentence.text = "";
        _NameTextBox.gameObject.SetActive(false);
        _Name.text = "";
        _ListOfCharacters.gameObject.SetActive(false);
        _ChoicesBox.SetActive(false);
    }

    /// <summary>
    /// Displays all dialogue-related UI elements.
    /// </summary>
    /// <param name="param">Optional parameter (not used).</param>
    private void ShowDialogueUI(object[] param)
    {
        _ContinueBtn.gameObject.SetActive(true);
        _TextBox.gameObject.SetActive(true);
        _MenuBackground.gameObject.SetActive(true);
        _Sentence.text = "";
        _NameTextBox.gameObject.SetActive(true);
        _Name.text = "";
        _ListOfCharacters.gameObject.SetActive(true);
    }

    /// <summary>
    /// Displays the choice box and fills it with options.
    /// </summary>
    /// <param name="param">Array containing dialogues and their labels.</param>
    private void ShowChoices(object[] param)
    {
        FillChoiceBox(param);
        _ChoicesBox.SetActive(true);
    }

    /// <summary>
    /// Hides the choice box UI element.
    /// </summary>
    public void HideChoices(object[] param)
    {
        _ChoicesBox.SetActive(false);

        for (int i = 0; i < _ChoicesContent.transform.childCount; i++)
        {
            Destroy(_ChoicesContent.transform.GetChild(i).gameObject);
        }
    }
}