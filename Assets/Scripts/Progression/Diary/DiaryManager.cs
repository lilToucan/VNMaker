using System.Collections.Generic;
using UnityEngine;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace VNMaker.Progression.Diary
{
    public class DiaryManager : MonoBehaviour
    {
        
#if UNITY_EDITOR
        // used in custom editor
        public ConditionItemMap ItemMap { get => _ItemMap; set => _ItemMap = value; } 
        [HideInInspector] public string ScriptableObjectsPath;
#endif
        // used in game
        [SerializeField] private ConditionItemMap _ItemMap;
        
        private void OnEnable()
        {
            GameManager.Instance.InteractableEvents.Register(InteractEventList.ON_CONDITION_CHANGE, OnItemListChanged);
        }

        private void OnDisable()
        {
            GameManager.Instance.InteractableEvents.Unregister(InteractEventList.ON_CONDITION_CHANGE, OnItemListChanged);
        }

        private void OnItemListChanged(params object[] param)
        {
            Conditions conditions = ConditionsUtils.GetSavedConditions(); // get the saved conditions
            List<ConditionToItemData> itemsChanged = new(); // used to save the conditions corresponding to items

            foreach (var condition in conditions) // for each condition
            {
                if (!_ItemMap.TryGetValue(condition.Key, out ItemDataSO itemData)) // if this is not an item skip it. (checks if the ConditionEnum is inside the list of items)
                    continue;

                if (!itemData.ObjectsStates.TryGetValue(condition.Value, out UiItemData itemModel)) // if there is no int value inside the ItemConditionsMap that matches the one inside the condition then skip 
                    continue;

                // add to the changed items
                itemsChanged.Add(new(condition.Key, itemModel));
            }

            // Change the diary UI:
            GameManager.Instance.InteractableEvents.TriggerEvent(InteractEventList.ON_DIARY_CHANGE, itemsChanged);
        }
    }
}