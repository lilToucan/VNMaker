using VNMaker.Progression;

namespace VNMaker.SaveSystem
{
    public class SaveData
    {
        public int CurrentSavedScene = 0;
        public Conditions CurrentSavedConditions;

        public SaveData()
        {
            CurrentSavedScene = 0;
            CurrentSavedConditions = new Conditions();
        }
    }
}