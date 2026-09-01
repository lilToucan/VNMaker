using UnityEngine;
using System.Collections;

namespace DialogueSystem
{
    public class DialogueSoundManager : MonoBehaviour
    {
        private AudioSource _audioSourceMusic1;
        private AudioSource _audioSourceMusic2;

        private AudioSource _audioSourceSfx;

        private AudioClip _currentSong;

        private float _currentStartingPoint;

        private AudioClip _nextSong;

        private float _nextStartingPoint;
        private double _currentAudioLength;

        private double _scheduledStartTime = 0;
        private bool _isNextScheduled = false;


        public float SecondsForFading = 1.5f;

        private void Awake()
        {
            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            AudioSource[] sources = GetComponents<AudioSource>();

            if (sources.Length < 3)
            {
                Debug.LogError($"ERROR AUDIO SOURCES MUST BE AT LEAST 3 \ncurrent number: {sources.Length}");
                return;
            }

            _audioSourceSfx = sources[0];
            
            _audioSourceMusic1 = sources[1];
            _audioSourceMusic2 = sources[2];
            
            _audioSourceMusic1.loop = false;
            _audioSourceMusic2.loop = false;
            _audioSourceMusic1.playOnAwake = false;
            _audioSourceMusic2.playOnAwake = false;
        }

        public void PlayOneShotSound(AudioClip clip)
        {
            _audioSourceSfx.PlayOneShot(clip);
        }

        public void PlayOnLoop(AudioClip clip, float startingPointLoop)
        {
            // if no other songs are playing then redo 
            if (_currentSong == null)
            {
                _currentSong = clip;
                _currentStartingPoint = startingPointLoop;
                AudioSource source = _audioSourceMusic1.isPlaying ? _audioSourceMusic1 : _audioSourceMusic2;
                source.clip = _currentSong;
                source.loop = true;
                source.Play();
                return;
            }

            if (clip == _currentSong) // if we are already playing this song then skip
                return;

            // if we're already playing something then crossfade between the 2 song
            _nextSong = clip;
            _nextStartingPoint = startingPointLoop;
            AudioSource currentSource = _audioSourceMusic1.isPlaying ? _audioSourceMusic1 : _audioSourceMusic2;
            AudioSource newSource = (currentSource == _audioSourceMusic1) ? _audioSourceMusic2 : _audioSourceMusic1;
            StartCoroutine(Crossfade(currentSource, newSource, SecondsForFading));
        }


        public void StopMusic()
        {
            ResetAudioSource(_audioSourceMusic1);
            ResetAudioSource(_audioSourceMusic2);
        }

        private IEnumerator Crossfade(AudioSource oldSource, AudioSource newSource, float duration)
        {
            newSource.clip = _nextSong;
            newSource.volume = 0;
            newSource.time = 0;
            newSource.loop = true;
            newSource.Play();

            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;

                oldSource.volume = Mathf.Lerp(1f, 0f, progress);
                newSource.volume = Mathf.Lerp(0f, 1f, progress);

                yield return null;
            }

            ResetAudioSource(oldSource);
            
            _currentSong = _nextSong;
            _currentStartingPoint = _nextStartingPoint;
            _nextSong = null;
            _nextStartingPoint = 0;
        }

        private void ResetAudioSource(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.loop = false;
            source.volume = 1f;
        }
    }
}