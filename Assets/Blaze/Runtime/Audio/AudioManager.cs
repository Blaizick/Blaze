using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Blaze.Runtime.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private Dictionary<AudioClip, AudioSource> m_SourcesDic = new();
        public AudioMixer mixer;

        public void Play(AudioClip clip, AudioMixerGroup group)
        {
            AudioSource s;
            if (!m_SourcesDic.TryGetValue(clip, out s))
            {
                s = gameObject.AddComponent<AudioSource>();
                s.clip = clip;
                m_SourcesDic[clip] = s;
            }
            s.Stop();
            s.outputAudioMixerGroup = group;
            s.Play();
        }
        public void Stop(AudioClip clip)
        {
            if (m_SourcesDic.TryGetValue(clip, out var s))
            {
                s.Stop();
            }
        }
        public void StopAll()
        {
            foreach (var (k, v) in m_SourcesDic)
            {
                v.Stop();
            }
        }

        public float GlobalVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("GlobalVolume", 20.0f);
            }
            set
            {
                mixer.SetFloat("GlobalVolume", value - 80);
                PlayerPrefs.SetFloat("GlobalVolume", value);
            }
        }
    }    
}