using VNMaker.Progression;

namespace VNMaker.SaveSystem
{
    public class SaveData
    {
        public int CurrentSavedScene = 0;
        public Conditions SavedConditionsMap;

        public SaveData()
        {
            CurrentSavedScene = 0;
            SavedConditionsMap = new Conditions();
        }
    }
}