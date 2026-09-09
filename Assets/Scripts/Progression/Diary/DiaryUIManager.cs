using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace VNMaker.Progression.Diary
{
    public class DiaryUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _DiaryUiPanel;

        [SerializeField] private GameObject _InventoryContentGameObject;

        [SerializeField] private Image _ItemPortrait;
        [SerializeField] private TextMeshProUGUI _Description;
        [SerializeField] private Sprite _ItemSlotDefaultSprite;

        [SerializeField] private GameObject _InventorySlotPrefab;

        private List<ConditionToItemData> _listOfItemsInDiary = new();
        private Dictionary<ConditionEnum, ItemSlot> _inventorySlots = new();

        private void OnEnable()
        {
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_DIARY_CHANGE, UpdateInventory);
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_ITEMSLOT_PRESSED, OnItemSlotPressed);

            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_CONDITION_CHANGE); // Calls to update inventory again just in case something went wrong 

            UpdateDetails(null, "");
        }


        private void OnDisable()
        {
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_DIARY_CHANGE, UpdateInventory);
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_ITEMSLOT_PRESSED, OnItemSlotPressed);
        }

        public void OpenCloseUI()
        {
            if (_DiaryUiPanel.activeInHierarchy)
                CloseUI();
            else
                OpenUI();
        }

        private void OpenUI()
        {
            LoadItemsInUI();
            _DiaryUiPanel.SetActive(true);
        }

        private void CloseUI()
        {
            _DiaryUiPanel.SetActive(false);
            UpdateDetails(null, "");
        }

        /// <summary>
        /// gets the list of items that changed from the diary manager and then checks if there are any errors
        /// </summary>
        /// <param name="param"></param>
        private void UpdateInventory(params object[] param)
        {
            List<ConditionToItemData> changedItems = (List<ConditionToItemData>)param[0];

            for (int x = 0; x < changedItems.Count; x++)
            {
                ConditionToItemData changedItemData = changedItems[x];

                for (int i = 0; i < _listOfItemsInDiary.Count; i++) // check if you already have the item saved then if so change just the data
                {
                    ConditionToItemData diaryItem = _listOfItemsInDiary[i];

                    if (!_inventorySlots.TryGetValue(changedItemData.ConditionsToUnlock, out ItemSlot inventorySlot))
                        continue;

                    if (changedItemData.ConditionsToUnlock != diaryItem.ConditionsToUnlock) // check if the changedItem already exist
                        continue;

                    _listOfItemsInDiary[i] = changedItemData; // if so then change that slot's data to the new one
                    changedItemData = null;
                    break;
                }

                if (changedItemData != null)
                    _listOfItemsInDiary.Add(changedItemData);
            }
        }

        /// <summary>
        /// iterates through the list of items in the diary <br></br>
        /// and Creates/Updates a slot with the data inside the iterated item <br></br>
        /// </summary>
        private void LoadItemsInUI()
        {
            foreach (ConditionToItemData diaryItemData in _listOfItemsInDiary)
            {
                if (diaryItemData == null)
                    continue;

                UiItemData uiItemData = diaryItemData.Data;

                if (_inventorySlots.TryGetValue(diaryItemData.ConditionsToUnlock, out ItemSlot inventorySlot)) // if already spawned then just update existing slot
                {
                    inventorySlot.UpdateData(uiItemData);
                    continue;
                }

                ItemSlot item = Instantiate(_InventorySlotPrefab, _InventoryContentGameObject.transform).ConvertTo<ItemSlot>();
                item.UpdateData(uiItemData);
                _inventorySlots.Add(diaryItemData.ConditionsToUnlock, item);
            }
        }

        private void OnItemSlotPressed(object[] obj)
        {
            ItemSlot itemSlot = (ItemSlot)obj[0];
            UpdateDetails(itemSlot.ItemImage, itemSlot.Description);
        }

        private void UpdateDetails(Sprite newSprite, string newDescription)
        {
            _ItemPortrait.sprite = newSprite;
            _Description.text = newDescription;
        }
    }
}