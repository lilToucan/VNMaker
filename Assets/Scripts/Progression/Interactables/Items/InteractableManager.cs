using UnityEngine;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace VNMaker.Interactables.Items
{
    public class InteractableManager : MonoBehaviour
    {
        private void OnEnable()
        {
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_CONDITION_CHANGE, RefreshInteractables);
        }

        private void OnDisable()
        {
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_CONDITION_CHANGE, RefreshInteractables);
        }

        void RefreshInteractables(params object[] param) 
        {
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.REFRESH_INTERACTABLE_PRE_CONDITION);
        }
    }
}