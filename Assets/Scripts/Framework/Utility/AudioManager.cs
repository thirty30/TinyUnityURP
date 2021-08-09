using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class AudioManager : MonoBehaviour
    {
        public AudioSource MusicAS;
        public AudioSource SoundAS;

        private static AudioManager mInstance;
        public static AudioManager GetSingleton() { return mInstance; }

        private Dictionary<string, AudioClip> mAudioDic = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this;
            }
        }

        public void Init(string aPath)
        {
            Object[] audios = AssetLoader.LoadAll(aPath);
            foreach (var audio in audios)
            {
                AudioClip ac = audio as AudioClip;
                this.mAudioDic.Add(ac.name, ac);
            }
        }

        public void PlayMusic(string a_strAudio)
        {
            AudioClip audio = null;
            this.mAudioDic.TryGetValue(a_strAudio, out audio);
            if (audio == null)
            {
                return;
            }
            if (this.MusicAS.clip == audio)
            {
                return;
            }
            if (this.MusicAS.isPlaying == true)
            {
                this.MusicAS.Stop();
            }
            this.MusicAS.loop = true;
            this.MusicAS.clip = audio;
            this.MusicAS.Play();
        }

        public void StopMusic()
        {
            this.MusicAS.Stop();
            this.MusicAS.clip = null;
        }

        public void SetMusicVolume(float a_Volume)
        {
            this.MusicAS.volume = a_Volume;
        }

        public void PlaySound(string a_strAudio)
        {
            AudioClip audio = null;
            this.mAudioDic.TryGetValue(a_strAudio, out audio);
            if (audio == null)
            {
                return;
            }
            this.SoundAS.loop = false;
            this.SoundAS.PlayOneShot(audio);
        }

        public void StopSound()
        {
            this.SoundAS.Stop();
        }

        public void SetSoundVolume(float a_Volume)
        {
            this.SoundAS.volume = a_Volume;
        }
    }
}
