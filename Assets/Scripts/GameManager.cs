using VNMaker.EventBuss;

namespace VNMaker.Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        private EventManager _dialogueEvents;
        private EventManager _interactableEvents;
        private EventManager _mapEvents;
        
        public EventManager DialogueEvents => _dialogueEvents;
        public EventManager InteractableEvents => _interactableEvents;
        public EventManager MapEvents => _mapEvents;

        protected override void Awake()
        {
            base.Awake();
            _dialogueEvents = new EventManager();
            _interactableEvents = new EventManager();
            _mapEvents = new EventManager();
            
        }
    }
}