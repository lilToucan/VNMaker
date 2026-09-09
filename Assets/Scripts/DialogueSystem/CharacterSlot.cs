using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using VNMaker.EventBuss;
using VNMaker.Singletons;

namespace DialogueSystem
{
    /// <summary>
    /// this class is used to play animations to the characters in the dialogue
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterSlot : MonoBehaviour
    {
        public SpriteRenderer SpriteHolder;
        public Animator AnimatorHolder;
        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixerPlayable;
        private AnimatorOverrideController _animatorOverrider;
        private AnimationClipPlayable _clipPlayable;

        private Vector3 _spriteStartPosition;

        private void Awake()
        {
            SpriteHolder = SpriteHolder == null ? GetComponentInChildren<SpriteRenderer>() : SpriteHolder;
            AnimatorHolder = AnimatorHolder == null ? GetComponent<Animator>() : AnimatorHolder;

            _graph = PlayableGraph.Create($"{gameObject.name}'s Graph"); // pulling out dark magic to make this work
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_graph, "Anim output", AnimatorHolder);

            _mixerPlayable = AnimationMixerPlayable.Create(_graph, 1);
            playableOutput.SetSourcePlayable(_mixerPlayable);

            AnimatorHolder.runtimeAnimatorController = null;
        }

        private void Start()
        {
            _spriteStartPosition = SpriteHolder.transform.localPosition;
        }

        private void OnEnable()
        {
            GameManager.Instance.DialogueEvents.Register(DialogueEventList.RESET_CHARACTER, ResetCharacter);
        }

        private void OnDisable()
        {
            
            GameManager.Instance.DialogueEvents.Unregister(DialogueEventList.RESET_CHARACTER, ResetCharacter);
        }

        /// <summary>
        /// Called by the dialogue elaborator to reset any changes the previous animation did
        /// </summary>
        /// <param name="obj">nothin</param>
        private void ResetCharacter(object[] obj)
        {
            SpriteHolder.transform.localScale = Vector3.one;
            SpriteHolder.transform.rotation = Quaternion.identity;
            SpriteHolder.transform.localPosition = _spriteStartPosition;

            for (int x = 0; x < _mixerPlayable.GetInputCount(); x++)
            {
                _mixerPlayable.DisconnectInput(x);
            }
        }

        /// <summary>
        /// Called by the DialogueUiManager in the ChangeImage function <br></br>
        /// resets all inputs then cycles through every given animation and connects them with the playable system <br></br>
        /// then it plays them all at once 
        /// </summary>
        /// <param name="animations">the animations to play</param>
        public void PlayAnimations(List<AnimationClip> animations)
        {
            if (animations.Count <= 0)
                return;

            for (int x = 0; x < _mixerPlayable.GetInputCount(); x++) // doing it again cause you never know
            {
                _mixerPlayable.DisconnectInput(x);
            }

            _mixerPlayable.SetInputCount(animations.Count);

            int i = 0;
            foreach (var clip in animations)
            {
                _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
                _mixerPlayable.ConnectInput(i, _clipPlayable, 0);
                _mixerPlayable.SetInputWeight(i, 1);
                i++;
            }

            _graph.Play();
        }

        /// <summary>
        /// since the playable system doesn't have garbage collection i have to do it
        /// </summary>
        private void OnDestroy()
        {
            if (_graph.IsValid())
                _graph.Destroy();

            // to be honest idk if i need to do this 2
            
            if (_mixerPlayable.IsValid())
                _mixerPlayable.Destroy();

            if (_clipPlayable.IsValid())
                _clipPlayable.Destroy();
        }
    }
}