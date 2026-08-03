using UnityEngine;
using UnityEngine.EventSystems;
using VNMaker.EventBuss;
using VNMaker.Progression;
using VNMaker.Singletons;

namespace VNMaker.Interactables.Items
{
    [RequireComponent(typeof(SpriteRenderer), typeof(EventTrigger))]
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] private Conditions _preConditions;
        [SerializeField] private Conditions _postConditions;

        private SpriteRenderer _spriteRenderer;
        private EventTrigger _eventTrigger;
        private EventTrigger.Entry _entry = new();

        public Conditions PreConditions =>  _preConditions;
        public Conditions PostConditions => _postConditions;

        private void OnEnable()
        {
            _eventTrigger = GetComponent<EventTrigger>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // start listening for the EventTrigger's OnClick
            _entry.eventID = EventTriggerType.PointerClick;
            _entry.callback.AddListener((eventData) => { ApplyPostConditions(); });
            _eventTrigger.triggers.Add(_entry);

            CheckPreConditions(); // checks if it should be visible or not

            GameManager.Instance.InteractableEvents.Register(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION, CheckPreConditions);
        }

        private void OnDisable()
        {
            _eventTrigger.triggers.Remove(_entry);
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION, CheckPreConditions);
        }

        /// <summary>
        /// applies the conditions <br></br>
        /// then triggers the ON_CONDITION_CHANGE event refreshing every interactable and the inventory UI 
        /// </summary>
        private void ApplyPostConditions()
        {
            ConditionsUtils.ApplyCondition(_postConditions);
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_CONDITION_CHANGE); 
        }

        /// <summary>
        /// checks if the item should be visible by the player
        /// </summary>
        /// <param name="param">no need to put anything here</param>
        private void CheckPreConditions(params object[] param)
        {
            // if the preconditions are not met deactivate 
            if (!ConditionsUtils.CheckConditions(_preConditions))
            {
                _spriteRenderer.enabled = false;
                _eventTrigger.enabled = false;
                return;
            }

            _spriteRenderer.enabled = true;
            _eventTrigger.enabled = true;
        }
    }
}