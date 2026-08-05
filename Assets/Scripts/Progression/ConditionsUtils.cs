using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VNMaker.Singletons;

namespace VNMaker.Progression
{
   
    public class ConditionsUtils
    {
        
        /// <summary>
        /// Gets a reference to the saved Conditions <br></br>
        /// Then it loops the given conditions checking if they aren't inside the savedConditions if so it returns false <br></br>
        /// if, instead, all the conditions checked are inside the savedConditions it returns true
        /// </summary>
        /// <param name="conditions">given conditions to check</param>
        /// <returns>Returns true when the given conditions are inside the SavedConditions</returns>
        public static bool CheckConditions(Conditions conditions)
        {
            Conditions savedConditions = GetSavedConditions();
            
            foreach (KeyValuePair<ConditionKeys, int> condition in conditions) 
            {
                if (!savedConditions.TryGetValue(condition.Key, out int value)) // if the key exists in the savedConditions
                    return false;

                if (value != condition.Value) // if the value of the key is the same as in the savedConditions (ex <key, 1> != <key, 2>)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a reference to the saved Conditions <br></br>
        /// Then it loops the Conditions given as parameter and saves them inside the saved conditions <br></br>
        /// Finally it saves the updated conditions to the file <br></br>
        /// </summary>
        public static void ApplyCondition(Conditions conditions)
        {
           Conditions savedConditions = GetSavedConditions();

            foreach (var condition in conditions)
            {
                if (savedConditions.ContainsKey(condition.Key))
                {
                    savedConditions[condition.Key] = condition.Value;
                }
                else
                {
                    savedConditions.Add(condition.Key, condition.Value);
                }
            }
            GameManager.Instance.SaveManager.SaveConditionsToFile(savedConditions, -1); // -1 to skip saving the scene
        }

        /// <summary>
        /// Gets the saved conditions inside the save file 
        /// </summary>
        /// <returns></returns>
        public static Conditions GetSavedConditions()
        {
            return GameManager.Instance.SaveManager.LoadSaveFile().SavedConditionsMap;
        }

        
    }
}
