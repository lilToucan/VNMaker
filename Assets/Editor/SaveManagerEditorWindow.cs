using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VNMaker.SaveSystem;

public class SaveManagerEditorWindow : EditorWindow
{
    // create a window with all the saved conditions 
    // and under a button to reset it 
    
    [MenuItem("Window/SaveManagerEdit")]
    public static void ShowWindow()
    {
        SaveManagerEditorWindow window = GetWindow<SaveManagerEditorWindow>("SaveManagerEdit");
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Create button
        Button button = new Button();
        button.name = "ResetConditions";
        button.text = "Reset Conditions";
        button.clicked += SaveManager.EditorResetConditionFile;
        root.Add(button);
    }
}
