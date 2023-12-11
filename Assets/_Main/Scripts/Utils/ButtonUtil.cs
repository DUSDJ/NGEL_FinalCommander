using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FC
{
    public enum EnumUISound
    {
        Sound_CommonButtonClick = 0,
        
        Sound_AddSample = 1,

    }

    public class ButtonUtil : MonoBehaviour
    {
        public EnumUISound UISound;


        /// <summary>
        /// 오디오매니저 통해 사운드 재생
        /// </summary>
        /// <param name="msg"></param>
        public void PlaySound(string msg)
        {
            AudioManager.Instance.PlayOneShot(msg);
        }


        public void PlaySoundByEnum()
        {
            AudioManager.Instance.PlayOneShot(UISound.ToString());
        }

    }
}