using DialogueSystem;
using UnityEngine.SceneManagement;
using VNMaker.EventBuss;
using VNMaker.SaveSystem;

namespace VNMaker.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        private EventManager _dialogueEvents;
        private EventManager _interactableEvents;
        private SaveManager _saveManager;
        private DialogueSoundManager _dialogueSoundManager;

        public EventManager DialogueEvents => _dialogueEvents;
        public EventManager InteractableEvents => _interactableEvents;
        public SaveManager SaveManager => _saveManager;
        
        public DialogueSoundManager DialogueSoundManager => _dialogueSoundManager;
        

        protected override void Awake()
        {
            base.Awake();
            _dialogueEvents = new EventManager();
            _interactableEvents = new EventManager();
            _saveManager = new SaveManager();

            _dialogueSoundManager = GetComponentInChildren<DialogueSoundManager>();
            

#if UNITY_EDITOR
            return;
#endif
            // saves the new scene
            SceneManager.activeSceneChanged += (sender, args) => { _saveManager.SaveConditionsToFile(null, SceneManager.GetActiveScene().buildIndex); };
        }
    }
}