using System.Collections.Generic;
using UnityEngine;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace VNMaker.Progression.Diary
{
    public class DiaryManager : MonoBehaviour
    {
        [SerializeField] private ConditionItemMap _itemMap;

#if UNITY_EDITOR
        public ConditionItemMap ItemMap { get => _itemMap; set => _itemMap = value; }
#endif

        private void OnEnable()
        {
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_CONDITION_CHANGE, OnItemListChanged);
        }

        private void OnDisable()
        {
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_CONDITION_CHANGE, OnItemListChanged);
        }

        public void OnItemListChanged(params object[] param)
        {
            // get the conditions the player has and then check if any of them are items and then change the diary UI
            Conditions conditions = ConditionsUtils.GetSavedConditions();
            List<DiaryUiItemData> itemsChanged = new();

            foreach (var pair in conditions)
            {
                if (!_itemMap.TryGetValue(pair.Key, out ItemDataSO itemData))
                    continue;

                if (!itemData.ObjectsStates.TryGetValue(pair.Value, out ItemModel itemModel))
                    return;

                // add to the changed items
                itemsChanged.Add(new(pair.Key, itemModel));
            }

            // Change the diary UI:
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_DIARY_CHANGE, itemsChanged);
        }
    }
}