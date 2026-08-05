using UnityEngine;

namespace VNMaker.Progression.Diary
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ItemData", order = 0)]
    public class ItemDataSO : ScriptableObject
    {
        public ItemConditionsMap ObjectConditions;
    }
}