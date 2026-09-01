using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DialogueSystem
{
    [RequireComponent(typeof(Animator))]
    public class CharacterSlot : MonoBehaviour
    {
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixerPlayable;
        public SpriteRenderer SpriteHolder;
        public Animator AnimatorHolder;
        private AnimatorOverrideController _animatorOverrider;
        private List<AnimationClipPlayable> _playableClips = new();

        private static int index;

        // for testing purposes 
        //[SerializeField] private List<AnimationClip> _animations;

        private void Awake()
        {
            SpriteHolder = SpriteHolder == null ? GetComponentInChildren<SpriteRenderer>() : SpriteHolder;
            AnimatorHolder = AnimatorHolder == null ? GetComponent<Animator>() : AnimatorHolder;

            _graph = PlayableGraph.Create($"{gameObject.name}'s Graph"); // pulling out dark magic to make this work
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_graph, "Anim output", AnimatorHolder);

            _mixerPlayable = AnimationMixerPlayable.Create(_graph, 1);
            playableOutput.SetSourcePlayable(_mixerPlayable);

            var controllerPlayable = AnimatorControllerPlayable.Create(_graph, AnimatorHolder.runtimeAnimatorController);
            _mixerPlayable.ConnectInput(0, controllerPlayable, 0);
            _mixerPlayable.SetInputWeight(0, 0);
        }
        
        // [Test]
        // public void TestingPlayAnims()
        // {
        //     if (_animations.Count <= 0)
        //         return;
        //     foreach (var clip in _playableClips)
        //     {
        //         clip.Destroy();
        //     }
        //
        //     _mixerPlayable.SetInputCount(_animations.Count + 1); // +1 cause i need to keep the Animator's runtimeAnimatorController
        //     int i = 1;
        //     foreach (var clip in _animations)
        //     {
        //         Debug.Log(clip.name);
        //         var clipPlayable = AnimationClipPlayable.Create(_graph, clip);
        //         _mixerPlayable.ConnectInput(i, clipPlayable, 0);
        //         _mixerPlayable.SetInputWeight(i, 1);
        //         _playableClips.Add(clipPlayable);
        //         i++;
        //     }
        //
        //     _graph.Play();
        // }

        public void PlayAnimations(List<AnimationClip> animations)
        {
            if (animations.Count <= 0)
                return;

            foreach (var clip in _playableClips)
            {
                clip.Destroy();
            }

            _mixerPlayable.SetInputCount(animations.Count + 1); // +1 cause i need to keep the Animator's runtimeAnimatorController
            int i = 1;
            foreach (var clip in animations)
            {
                var clipPlayable = AnimationClipPlayable.Create(_graph, clip);
                _mixerPlayable.ConnectInput(i, clipPlayable, 0);
                _mixerPlayable.SetInputWeight(i, 1);
                _playableClips.Add(clipPlayable);
                i++;
            }

            _graph.Play();
        }
        
        private void OnDestroy()
        {
            if (_graph.IsValid())
                _graph.Destroy();

            if (_mixerPlayable.IsValid())
                _mixerPlayable.Destroy();

            if (_playableClips.Count > 0)
            {
                foreach (var clip in _playableClips)
                {
                    if (clip.IsValid())
                        clip.Destroy();
                }
            }
        }

    }
}