namespace VNMaker.Progression.Diary
{
    public class ConditionToItemData
    {
        public ConditionEnum ConditionsToUnlock;
        public UiItemData Data;

        public ConditionToItemData(ConditionEnum conditionsToUnlock, UiItemData data)
        {
            this.ConditionsToUnlock = conditionsToUnlock;
            this.Data = data;
        }
    }
}