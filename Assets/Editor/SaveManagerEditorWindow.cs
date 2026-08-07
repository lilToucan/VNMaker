using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VNMaker.Progression;
using VNMaker.SaveSystem;

public class SaveManagerEditorWindow : EditorWindow
{
    // create a window with all the saved conditions 
    // and under a button to reset it 
    // could also add that every condition can be deleted (probably could just put a Conditions property in and then save that one with a -1 for map)
    
    private SaveData _savedData;
    
    [MenuItem("Window/SaveManagerEdit")]
    public static void ShowWindow()
    {
        SaveManagerEditorWindow window = GetWindow<SaveManagerEditorWindow>("SaveManagerEdit");
    }

    private void OnBecameVisible()
    {
        _savedData = SaveManager.LoadSaveFileEditor();
    }

    private void CreateGUI()
    {
        
        VisualElement root = rootVisualElement;
        
        // Reset save file button
        Button button = new Button();
        button.name = "ResetConditions";
        button.text = "Reset Conditions";
        button.clicked += SaveManager.EditorResetConditionFile;
        root.Add(button);
    }
}
