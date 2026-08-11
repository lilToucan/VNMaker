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
        [SerializeField] private GameObject diaryUiPanel;
        
        [SerializeField] private GameObject InventoryContentGameObject;

        [SerializeField] private Image itemPortrait;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Sprite itemSlotDefaultSprite;

        [SerializeField] private GameObject InventorySlotPrefab;
        // [SerializeField] private ItemSlot[] _itemSlots = new ItemSlot[16];

        private List<DiaryUiItemData> _itemsList = new();
        private List<DiaryUiItemData> _charactersList = new();
        private List<DiaryUiItemData> _locationsList = new();

        private int _currentPageIndex = 0;


        private void OnEnable()
        {
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_DIARY_CHANGE, UpdateDiaryUI);
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_ITEMSLOT_PRESSED, OnItemSlotPressed);
        }


        private void OnDisable()
        {
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_DIARY_CHANGE, UpdateDiaryUI);
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_ITEMSLOT_PRESSED, OnItemSlotPressed);
        }

        public void OpenUI()
        {
            LoadItemsInUI(ref _itemsList);

            diaryUiPanel.SetActive(true);
        }

        public void CloseUI()
        {
            diaryUiPanel.SetActive(false);
        }

        /// <summary>
        /// get's the list of items that changed from the diary manager and then checks if there are any errors
        /// </summary>
        /// <param name="param"></param>
        private void UpdateDiaryUI(params object[] param)
        {
            List<DiaryUiItemData> changedItems = (List<DiaryUiItemData>)param[0];

            foreach (var changedItem in changedItems)
            {
                CheckDuplicateInsideList(changedItem, ref _itemsList);
            }

            void CheckDuplicateInsideList(DiaryUiItemData changedItem, ref List<DiaryUiItemData> itemList)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (changedItem.ConditionsToUnlock != itemList[i].ConditionsToUnlock)
                        continue;

                    itemList[i] = changedItem;
                    return;
                }

                itemList.Add(changedItem);
                return;
            }
        }

        private void LoadItemsInUI(ref List<DiaryUiItemData> items)
        { for (int i = 0; i < items.Count; i++)
            {
                
                ItemModel listItem = items[i].Model;
                ItemSlot item = Instantiate(InventorySlotPrefab, InventoryContentGameObject.transform).ConvertTo<ItemSlot>();
                item.Description = listItem.Description;
                item.ItemImage = listItem.ObjectImage;
                item.Icon.sprite = listItem.ObjectIcon;
            }
            
        }
        
        private void OnItemSlotPressed(object[] obj)
        {
            ItemSlot itemSlot = (ItemSlot)obj[0];

            itemPortrait.sprite = itemSlot.ItemImage;
            description.text = itemSlot.Description;
        }
    }
}