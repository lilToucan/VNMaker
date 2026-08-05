namespace VNMaker.Progression.Diary
{
    public class DiaryUiItemData
    {
        public ConditionKeys ConditionsToUnlock;
        public ItemModel Model;

        public DiaryUiItemData(ConditionKeys conditionsToUnlock, ItemModel model)
        {
            this.ConditionsToUnlock = conditionsToUnlock;
            this.Model = model;
        }
    }
}