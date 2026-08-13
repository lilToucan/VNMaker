using VNMaker.EventBuss;
using VNMaker.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VNMaker.Progression.Diary
{
    public class ItemSlot : MonoBehaviour
    {
        // item slot Icon, name and button
        public Image Icon;
        public TextMeshProUGUI SlotName;
        [SerializeField] private Button _Button;

        // general item and description
        [HideInInspector] public Sprite ItemImage;
        [HideInInspector] public string Description;

        private void OnEnable()
        {
            _Button.onClick.AddListener(new(OnInteraction));
        }

        private void OnDisable()
        {
            _Button.onClick.RemoveAllListeners();
        }

        public void UpdateData(UiItemData newData)
        {
            Description =  newData.Description != "" ? newData.Description : Description ;
            ItemImage = newData.ObjectImage != null ? newData.ObjectImage : ItemImage;
            Icon.sprite = newData.ObjectIcon !=  null ? newData.ObjectIcon :  Icon.sprite;
            SlotName.text = newData.Name != "" ? newData.Name : SlotName.text;
        }

        public void OnInteraction()
        {
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_ITEMSLOT_PRESSED, this);
        }
    }
}