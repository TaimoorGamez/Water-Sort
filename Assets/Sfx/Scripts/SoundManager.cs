using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections.Generic;

namespace ProjectCore.Sfx
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] SOEvents OnOffBGMusic, OnOffSounds, PlayBtnClick, StopLoopSoundEffect;
        [SerializeField] SOIntegerEvents SoundEffectEvent, LoopEffectsEvent;
        [SerializeField] DBInt Music, Sound;
        [SerializeField] AudioClip BgMusic, BtnClick;
        [SerializeField] AudioClip[] EffectClips, LoopClips;

        AudioSource _bgSource = null, _btnSource = null, _effectSource = null, _loopSource = null;
        Stack<AudioSource> _walkSources = new Stack<AudioSource>();
        float _bgVolume = 0.6f;

        private void OnEnable()
        {
            OnOffBGMusic.EventHandler += ChangeBGMusicState;
            OnOffSounds.EventHandler += ChangeSoundState;
            PlayBtnClick.EventHandler += PlayBtnSound;
            SoundEffectEvent.EventHandler += PlaySoundEffect;
            LoopEffectsEvent.EventHandler += PlayLoopSounds;
            StopLoopSoundEffect.EventHandler += StopLoopSounds;
        }

        private void OnDisable()
        {
            OnOffBGMusic.EventHandler -= ChangeBGMusicState;
            OnOffSounds.EventHandler -= ChangeSoundState;
            PlayBtnClick.EventHandler -= PlayBtnSound;
            SoundEffectEvent.EventHandler -= PlaySoundEffect;
            LoopEffectsEvent.EventHandler -= PlayLoopSounds;
            StopLoopSoundEffect.EventHandler -= StopLoopSounds;
        }

        private void Start()
        {
            if (Music.Value == 1)
            {
                CreateMusicSources();
            }

            if (Sound.Value == 1)
            {
                CreateSoundSources();
            }
        }

        void CreateMusicSources()
        {
            if (_bgSource == null)
            {
                _bgSource = gameObject.AddComponent<AudioSource>();
                PlayBGMusic();
            }
        }

        void ChangeBGMusicState()
        {
            if (Music.Value == 1)
            {
                Music.Value = 0; 
                if (_bgSource != null)
                {
                    Destroy(_bgSource);
                }
            }
            else
            {
                Music.Value = 1;
                CreateMusicSources();
            }
        }

        void PlayBGMusic()
        {
            if (Music.Value == 1)
            {
                _bgSource.Stop();
                _bgSource.volume = _bgVolume;
                _bgSource.clip = BgMusic;
                _bgSource.loop = true;
                _bgSource.Play();
            }
        }
        void CreateSoundSources()
        {
            if (_btnSource == null)
            {
                _btnSource = gameObject.AddComponent<AudioSource>();
                _btnSource.clip = BtnClick;
            }

            if (_effectSource == null)
            {
                _effectSource = gameObject.AddComponent<AudioSource>();
            }

            if (_loopSource == null)
            {
                _loopSource = gameObject.AddComponent<AudioSource>();
                _loopSource.loop = true;
            }
        }

        void ChangeSoundState()
        {
            if (Sound.Value == 1)
            {
                Sound.Value = 0;
                if (_btnSource != null)
                {
                    Destroy(_btnSource);
                }

                if (_effectSource != null)
                {
                    Destroy(_effectSource);
                }

                if (_loopSource != null)
                {
                    Destroy(_loopSource);
                }
            }
            else
            {
                Sound.Value = 1;
                CreateSoundSources();
            }
        }

        void PlayBtnSound()
        {
            if (Sound.Value == 1)
            {
                _btnSource.Play();
            }
        }
    
        void PlaySoundEffect(int effectNum)
        {
            if (Sound.Value == 1)
            {
                if (_effectSource.isPlaying)
                {
                    AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
                    tempAudioSource.clip = EffectClips[effectNum];

                    // Play the sound
                    tempAudioSource.Play();

                    // Destroy the AudioSource after the sound has finished playing
                    Destroy(tempAudioSource, EffectClips[effectNum].length);
                }
                else
                {
                    _effectSource.PlayOneShot(EffectClips[effectNum]);
                }
            }
        }

        void PlayLoopSounds(int effectNum)
        {
            if (Sound.Value == 1)
            {
                {
                    _loopSource.clip = LoopClips[effectNum];
                    _loopSource.Play();
                }
            }
        }

        void StopLoopSounds()
        {
            _loopSource.Stop();
        }
    }
}
