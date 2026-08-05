using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace VNMaker.Progression.Diary
{
    [System.Serializable]
    public struct ItemModel
    {
        public string sName;
        [TextArea]
        public string Description;
        public Sprite ObjectIcon;
        public Sprite ObjectImage;
    }

    public class ItemConditionsMap : SerializedDictionary<int,ItemModel>
    { }
}