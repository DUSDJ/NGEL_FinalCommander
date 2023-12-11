using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FC
{

    public class AudioManager : MonoBehaviour
    {
        #region SingleTon

        /* SingleTon */
        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();

                    if (!_instance)
                    {
                        var prefab = Resources.Load<AudioManager>("Manager/AudioManager");
                        _instance = Instantiate(prefab);

                        if (!_instance)
                        {
                            Debug.LogError("AudioManager is null");
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        public Dictionary<string, AudioClip> AudioDic;

        private AudioSource audioSource;
        private AudioSource audioSourceBGM;

        private bool isInit = false;


        #region Awake & Init


        private void Awake()
        {
            #region SingleTon

            if (Instance == this)
            {
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);

                return;
            }

            #endregion

        }



        public IEnumerator InitRoutine()
        {
            if (isInit)
            {
                yield break;
            }

            Debug.Log("=====AudioManager Init=====");

            AudioDic = new Dictionary<string, AudioClip>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (audioSourceBGM == null)
            {
                audioSourceBGM = gameObject.AddComponent<AudioSource>();
            }

            #region Resources Load

            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/BGM");
            for (int i = 0; i < clips.Length; i++)
            {
                Debug.LogFormat("AudioDic Add : {0}", clips[i].name);
                AudioDic.Add(clips[i].name, clips[i]);
            }


            AudioClip[] clips_efx = Resources.LoadAll<AudioClip>("Audio/SE");
            for (int i = 0; i < clips_efx.Length; i++)
            {
                Debug.LogFormat("AudioDic Add : {0}", clips_efx[i].name);
                AudioDic.Add(clips_efx[i].name, clips_efx[i]);
            }


            #endregion
            isInit = true;
        }

        #endregion



        #region Play 

        public void PlayOneShot(string msg)
        {
            if (AudioDic == null)
            {
                return;
            }

            if (AudioDic.ContainsKey(msg) == false)
            {
                Debug.LogError(string.Format("Key : {0} 오디오소스가 없음", msg));
                return;
            }

            audioSource.PlayOneShot(AudioDic[msg]);
        }

        #endregion





        #region BGM

        public void SetBGM(string msg)
        {
            if (audioSourceBGM == null)
            {
                Debug.LogError("audioSourceBGM null");
                return;
            }

            if (AudioDic.ContainsKey(msg) == false)
            {
                Debug.LogError(string.Format("Key : {0} 오디오소스가 없음", msg));
                return;
            }

            if (audioSourceBGM.clip == null
                || audioSourceBGM.clip != AudioDic[msg])
            {
                audioSourceBGM.Stop();
                audioSourceBGM.clip = AudioDic[msg];
                audioSourceBGM.loop = true;
                audioSourceBGM.Play();
                return;
            }

            // audioSourceBGM.clip = AudioDic[msg];
            // audioSourceBGM.loop = true;
            // audioSourceBGM.Play();
        }

        public void BGMStop()
        {
            audioSourceBGM.Stop();
        }

        #endregion



        #region Volume Control

        public void SetEFXVolume(float value)
        {
            audioSource.volume = Mathf.Clamp01(value);

            PlayerPrefs.SetFloat("EfxVolume", value);
        }

        public float GetEFXVolume()
        {
            return audioSource.volume;
        }

        public void SetBGMVolume(float value)
        {
            audioSourceBGM.volume = Mathf.Clamp01(value);

            PlayerPrefs.SetFloat("BgmVolume", value);
        }

        public float GetBGMVolume()
        {
            return audioSourceBGM.volume;
        }

        #endregion

    }


}

