using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace DialogueSystem
{
    [System.Serializable]
    public class SentenceClass
    {
        [SerializeField] private string _Sentence;
        [SerializeField] private AudioClip _SFXAudio;

        [SerializeField] private SerializedDictionary<Sprite, List<AnimationClip>> _SpriteAnimMapAnimMap;

        public string Sentence => _Sentence;
        public AudioClip SFXAudio => _SFXAudio;
        public SerializedDictionary<Sprite, List<AnimationClip>> SpriteAnimMap => _SpriteAnimMapAnimMap;

        public SentenceClass(string sentence, SerializedDictionary<Sprite, List<AnimationClip>> spriteAnimMapAnimMap, AudioClip audio)
        {
            _Sentence = sentence;
            _SpriteAnimMapAnimMap = spriteAnimMapAnimMap;
            _SFXAudio = audio;
        }
    }
}