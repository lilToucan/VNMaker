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
            Conditions conditions = ConditionsUtils.GetSavedConditions(); // get the saved conditions
            List<DiaryUiItemData> itemsChanged = new(); // used to save the conditions corresponding to items

            foreach (var condition in conditions) // for each condition
            {
                if (!_itemMap.TryGetValue(condition.Key, out ItemDataSO itemData)) // if the conditionKey inside the condition is not inside the list of items then skip
                    continue;

                if (!itemData.ObjectsStates.TryGetValue(condition.Value, out ItemModel itemModel)) // if there is no int value inside the ItemConditionsMap that matches the one inside the condition then skip 
                    continue;

                // add to the changed items
                itemsChanged.Add(new(condition.Key, itemModel));
            }

            // Change the diary UI:
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_DIARY_CHANGE, itemsChanged);
        }
    }
}