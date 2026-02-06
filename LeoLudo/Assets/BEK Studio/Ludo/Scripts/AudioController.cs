using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BEKStudio {
    public class AudioController : MonoBehaviour {
        public static AudioController Instance;
        public AudioSource pawnMoveAudioSource;
        public AudioClip pawnMoveClip;
        public AudioSource diceAudioSource;
        public AudioClip diceClip;
        public AudioSource buttonAudioSource;
        public AudioClip buttonClip;
        public AudioSource musicAudioSource;
        public AudioClip musicAudioClip;

        [Header("SFX Effect")]
        public bool isSFXEnable = true;
        private const string SFX_PREF_KEY = "SFX_ENABLED";

        [Header("BG Music")]
        public bool isMusicEnable = true;
        private const string MUSIC_PREF_KEY = "SFX_ENABLED";

        void Awake() {
            if (Instance == null) {
                Instance = this;
                isSFXEnable = PlayerPrefs.GetInt(SFX_PREF_KEY, 1) == 1;
                isMusicEnable = PlayerPrefs.GetInt(MUSIC_PREF_KEY, 1) == 1;
                
                musicAudioSource.clip = musicAudioClip;
                musicAudioSource.loop = true;

                if (isMusicEnable)
                    musicAudioSource.Play();
                else
                    musicAudioSource.Stop();
            }
        }

        public void SetSFXEnable(bool isEnable) {
            isSFXEnable = isEnable;
            PlayerPrefs.SetInt(SFX_PREF_KEY, isSFXEnable ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetMusicEnable(bool isEnable) {
            isMusicEnable = isEnable;
            PlayerPrefs.SetInt(MUSIC_PREF_KEY, isMusicEnable ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void PlayBGMusic() 
        {
            if (musicAudioSource == null) return;
            if (musicAudioClip == null) return;

            if(isMusicEnable)
            {
                musicAudioSource.Play();
            }
            else
            {
                musicAudioSource.Stop();
                return;
            }
        }

        public void PlayPawnMoveSound() {
            if (!isSFXEnable) return;
            if (pawnMoveAudioSource == null) return;
            if (pawnMoveClip == null) return;

            if (pawnMoveAudioSource.isPlaying) pawnMoveAudioSource.Stop();
            pawnMoveAudioSource.PlayOneShot(pawnMoveClip);
        }

        public void PlayDiceSound() {
            if (!isSFXEnable) return;
            if (diceAudioSource == null) return;
            if (diceClip == null) return;

            if (diceAudioSource.isPlaying) diceAudioSource.Stop();
            diceAudioSource.PlayOneShot(diceClip);
        }

        public void PlayButtonSound() {
            if (!isSFXEnable) return;
            if (buttonAudioSource == null) return;
            if (buttonClip == null) return;

            if (buttonAudioSource.isPlaying) buttonAudioSource.Stop();
            buttonAudioSource.PlayOneShot(buttonClip);
        }
    }
}