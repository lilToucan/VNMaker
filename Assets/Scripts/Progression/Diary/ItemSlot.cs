using VNMaker.EventBuss;
using VNMaker.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VNMaker.Progression.Diary
{
    public class ItemSlot : MonoBehaviour  //diocane
    {
        
        // item slot Icon, name and button
        public Image Icon;
        public TextMeshProUGUI Name;
        [SerializeField] private Button _button;

        // general item and description
        [HideInInspector] public Sprite ItemImage;
        [HideInInspector] public string Description;

        private void OnEnable()
        {
            _button.onClick.AddListener(new(OnInteraction));
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }


        public void OnInteraction()
        {
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_ITEMSLOT_PRESSED, this);
        }

    }
}