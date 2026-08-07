using UnityEngine;

namespace VNMaker.Progression.Diary
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ItemData", order = 0)]
    public class ItemDataSO : ScriptableObject
    {
#if UNITY_EDITOR
        public ConditionKeys ObjectCondition;
#endif
        
        public ItemConditionsMap ObjectsStates; // basically all the possible states an object can have (ex: knife 1 = the knife is somewhere -> knife 2 = i have the knife -> knife 78 = the knife is now a stellar blade form the Xorp galaxy)
        
    }
}