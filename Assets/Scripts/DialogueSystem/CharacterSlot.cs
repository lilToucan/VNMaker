using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using VNMaker.EventBuss;
using VNMaker.Singletons;

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
        private AnimationClipPlayable _clipPlayable;

        private Vector3 _spriteStartPosition;

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

            // var controllerPlayable = AnimatorControllerPlayable.Create(_graph, AnimatorHolder.runtimeAnimatorController);
            // _mixerPlayable.ConnectInput(0, controllerPlayable, 0);
            // _mixerPlayable.SetInputWeight(0, 0);
            
            AnimatorHolder.runtimeAnimatorController = null;
        }


        private void Start()
        {
            _spriteStartPosition = SpriteHolder.transform.localPosition;
            GameManager.Instance.DialogueEvents.Register(DialogueEventList.RESET_CHARACTER, ResetCharacter);
        }

        private void ResetCharacter(object[] obj)
        {
            SpriteHolder.transform.localScale = Vector3.one;
            SpriteHolder.transform.rotation = Quaternion.identity;
            SpriteHolder.transform.localPosition = _spriteStartPosition;
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

            for (int x = 1; x < _mixerPlayable.GetInputCount(); x++)
            {
                _mixerPlayable.DisconnectInput(x);
            }

            _mixerPlayable.SetInputCount(animations.Count + 1); // +1 cause i need to keep the Animator's runtimeAnimatorController

            int i = 1; // again cause the 1st is the animator's runtimeAnimatorController
            foreach (var clip in animations)
            {
                _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
                _clipPlayable.GetAnimationClip().wrapMode = WrapMode.Once;
                _mixerPlayable.ConnectInput(i, _clipPlayable, 0);
                _mixerPlayable.SetInputWeight(i, 1);
                i++;
            }

            //_mixerPlayable.SetDone(false);
            _graph.Play();
            //_graph.Evaluate();
        }

        private void OnDestroy()
        {
            if (_graph.IsValid())
                _graph.Destroy();

            if (_mixerPlayable.IsValid())
                _mixerPlayable.Destroy();

            if (_clipPlayable.IsValid())
                _clipPlayable.Destroy();
        }
    }
}