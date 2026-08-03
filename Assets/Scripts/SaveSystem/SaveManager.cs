using System.IO;
using UnityEngine;
using VNMaker.Progression;

namespace VNMaker.SaveSystem
{
    public class SaveManager
    {
        private const string FileName = "Conditions.txt";

        /// <summary>
        /// Gets the path to the save file <br></br>
        /// Then if it exists, it returns the saved data <br></br>
        /// and if not, then it returns a new save  <br></br>
        /// </summary>
        public SaveData LoadSaveFile()
        {
            string path = Path.Combine(Application.persistentDataPath, FileName);
            SaveData saveData;

            if (File.Exists(path))
            {
                string data = File.ReadAllText(path);
                saveData = JsonUtility.FromJson<SaveData>(data);
            }
            else
                saveData = new SaveData();

            return saveData;
        }
        
        /// <summary>
        /// Gets a reference of the current saved data from the save file <br></br>
        /// if the id of the current scene is valid it saves the new value
        /// if the given conditions are valid it saves them
        /// </summary>
        /// <param name="conditions">the conditions to save (null = not saved)</param>
        /// <param name="currentScene">the id of the current scene (-1 = not saved)</param>
        public void SaveConditionsToFile(Conditions conditions, int currentScene)
        {
            SaveData saveData = LoadSaveFile();
            if(currentScene >= 0)
            {
                saveData.CurrentSavedScene = currentScene;
            }
            if(conditions != null)
            {
                saveData.SavedConditionsMap = conditions;
            }

            string serializedObject = JsonUtility.ToJson(saveData);
            string path = Path.Combine(Application.persistentDataPath, FileName);

            File.WriteAllText(path, serializedObject);
        }
        /// <summary>
        /// Deletes all saved data saved in the SaveFile
        /// </summary>
        public void ResetConditionFile()
        {
            SaveData saveData = new SaveData();
            
            string serializedObject = JsonUtility.ToJson(saveData);
            string path = Path.Combine(Application.persistentDataPath, FileName);

            File.WriteAllText(path, serializedObject);
        }
    }
}