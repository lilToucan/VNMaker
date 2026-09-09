using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class MonologueClass
    {
        [SerializeField] private string _Name;
        [SerializeField] private List<SentenceClass> _Sentences;

        public string Name => _Name;
        public List<SentenceClass> Sentences => _Sentences;

        public MonologueClass(string name, List<SentenceClass> sentences)
        {
            _Name = name;
            _Sentences = sentences;
        }
    }
}