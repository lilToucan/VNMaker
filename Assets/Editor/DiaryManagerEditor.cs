using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VNMaker.Progression.Diary;

namespace Editor
{
    [CustomEditor(typeof(DiaryManager))]
    public class DiaryManagerEditor : UnityEditor.Editor
    {
        private string scriptableObjectsPath;

        // public override VisualElement CreateInspectorGUI()
        // {
        //     VisualElement root = new();
        //     DiaryManager manager = (DiaryManager)target;
        //     
        //     Label PathLable = new("Scriptable objects path:");
        //     TextFi
        //     root.Add(PathLable);
        //     
        //     return root;
        // }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DiaryManager manager = (DiaryManager)target;

            scriptableObjectsPath = EditorGUILayout.TextField("Scriptable objects path: ", scriptableObjectsPath);

            if (GUILayout.Button("get all scriptable objects"))
            {
                string actualPath = scriptableObjectsPath != "" ? scriptableObjectsPath : "Assets/";
                
                // finds all scriptable objects in the given path then turns them into ItemDataSo
                ItemDataSO[] resources = AssetDatabase.FindAssets
                ("t:scriptableobject", new[] { actualPath })
                .Select(foundString => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(foundString))).ToArray();

                foreach (ItemDataSO itemData in resources)
                {
                    if (itemData== null || (manager.ItemMap.Count <= 0 && manager.ItemMap.ContainsKey(itemData.ObjectCondition)))
                        continue;

                    manager.ItemMap.Add(itemData.ObjectCondition, itemData);
                }
            }
        }
    }
}