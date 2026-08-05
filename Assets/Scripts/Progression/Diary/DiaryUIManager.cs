using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace VNMaker.Progression.Diary
{
    public class DiaryUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject diaryUiPanel;

        [SerializeField] private Image itemPortrait;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Sprite itemSlotDefaultSprite;

        [SerializeField] private ItemSlot[] _itemSlots = new ItemSlot[16];

        private List<DiaryUiItemData> _itemsList = new();
        private List<DiaryUiItemData> _charactersList = new();
        private List<DiaryUiItemData> _locationsList = new();

        private int _currentPageIndex = 0;


        private void OnEnable()
        {
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_DIARY_CHANGE, UpdateDiary);
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_ITEMSLOT_PRESSED,
                OnItemSlotPressed);
        }


        private void OnDisable()
        {
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_DIARY_CHANGE, UpdateDiary);
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_ITEMSLOT_PRESSED,
                OnItemSlotPressed);
        }

        public void OpenUI()
        {
            LoadUI(ref _itemsList);

            diaryUiPanel.SetActive(true);
        }

        public void CloseUI()
        {
            diaryUiPanel.SetActive(false);
        }

        private void UpdateDiary(params object[] param)
        {
            List<DiaryUiItemData> paramMap = (List<DiaryUiItemData>)param[0];

            foreach (var paramItem in paramMap)
            {
                CheckDuplicateInsideList(paramItem, ref _itemsList);
            }

            void CheckDuplicateInsideList(DiaryUiItemData paramItem, ref List<DiaryUiItemData> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (paramItem.ConditionsToUnlock != list[i].ConditionsToUnlock)
                        continue;

                    list[i] = paramItem;
                    return;
                }

                list.Add(paramItem);
                return;
            }
        }

        private void LoadUI(ref List<DiaryUiItemData> list)
        {
            // basicly the first index in the list that apears in the slots 
            // ex: slots.lengh = 2 list.count = 6, if pageIndex = 2 then we need the item in the list[4] slot 
            var firstIndexOnPage = _itemSlots.Length * _currentPageIndex;

            #region |loop the pages|

            if (firstIndexOnPage >= list.Count)
            {
                _currentPageIndex = 0;
                firstIndexOnPage = _itemSlots.Length * _currentPageIndex;
            }

            else if (firstIndexOnPage < 0)
            {
                _currentPageIndex = list.Count / _itemSlots.Length;
                firstIndexOnPage = _itemSlots.Length * _currentPageIndex;
            }

            #endregion |loop the pages|


            int itemSlotIndex = 0;

            for (int i = firstIndexOnPage; i < list.Count; i++)
            {
                if (i >= list.Count || itemSlotIndex >= _itemSlots.Length)
                    break;

                ItemModel listItem = list[i].Model;

                // change the icons of the items
                _itemSlots[itemSlotIndex].Icon.sprite = listItem.ObjectIcon;
                _itemSlots[itemSlotIndex].Name.text = listItem.sName;
                _itemSlots[itemSlotIndex].ItemImage = listItem.ObjectImage;
                _itemSlots[itemSlotIndex].Description = listItem.Description;

                itemSlotIndex++;
            }

            if (itemSlotIndex >= _itemSlots.Length)
                return;

            for (int i = itemSlotIndex; i < _itemSlots.Length; i++)
            {
                _itemSlots[i].Icon.sprite = itemSlotDefaultSprite;
                _itemSlots[i].Name.text = "";
                _itemSlots[i].ItemImage = null;
                _itemSlots[i].Description = "";
            }
        }

        /// <summary>
        /// changes the item loaded
        /// </summary>
        /// <param name="pageTurnDirection">-1 = previous page | 1 = next page </param>
        public void ChangePage(int pageTurnDirection)
        {
            _currentPageIndex += pageTurnDirection;


            LoadUI(ref _itemsList);
        }

        private void OnItemSlotPressed(object[] obj)
        {
            ItemSlot itemSlot = (ItemSlot)obj[0];

            itemPortrait.sprite = itemSlot.ItemImage;
            description.text = itemSlot.Description;
        }
    }
}