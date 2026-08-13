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
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DiaryManager manager = (DiaryManager)target;

            manager.ScriptableObjectsPath = EditorGUILayout.TextField("Scriptable objects path: ", manager.ScriptableObjectsPath);

            if (GUILayout.Button("get all scriptable objects"))
            {
                string actualPath = manager.ScriptableObjectsPath != "" ? "Assets/" + manager.ScriptableObjectsPath : "Assets/";

                // finds all scriptable objects in the given path then turns them into ItemDataSo
                ItemDataSO[] resources = AssetDatabase.FindAssets
                        ("t:scriptableobject", new[] { actualPath })
                    .Select(foundString => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(foundString))).ToArray();

                foreach (ItemDataSO itemData in resources)
                {
                    if
                    (
                        itemData == null
                        || manager.ItemMap.ContainsKey(itemData.ObjectCondition)
                    )
                        continue;

                    manager.ItemMap.Add(itemData.ObjectCondition, itemData);
                }
            }
        }
    }
}