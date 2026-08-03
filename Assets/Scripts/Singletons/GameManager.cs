using VNMaker.EventBuss;
using VNMaker.SaveSystem;

namespace VNMaker.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        private EventManager _dialogueEvents;
        private EventManager _interactableEvents;
        private EventManager _mapEvents;
        private SaveManager _saveManager;
        
        public EventManager DialogueEvents => _dialogueEvents;
        public EventManager InteractableEvents => _interactableEvents;
        public EventManager MapEvents => _mapEvents;
        public SaveManager SaveManager => _saveManager;

        protected override void Awake()
        {
            base.Awake();
            _dialogueEvents = new EventManager();
            _interactableEvents = new EventManager();
            _mapEvents = new EventManager();
            _saveManager = new SaveManager();
        }
    }
}