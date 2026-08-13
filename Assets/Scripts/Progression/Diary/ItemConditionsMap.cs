using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace VNMaker.Progression.Diary
{
    [System.Serializable]
    public struct UiItemData
    {
        public string Name;
        [TextArea]
        public string Description;
        public Sprite ObjectIcon;
        public Sprite ObjectImage;
    }
    [System.Serializable]
    public class ItemConditionsMap : SerializedDictionary<int,UiItemData>
    { }
}