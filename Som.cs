using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Som : MonoBehaviour
{
    public static AudioSource aus;
    private void Awake()
    {
        aus = GetComponent<AudioSource>();
    }
    public static void S(string clip, float volum = 1)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(clip);
        aus.PlayOneShot(audioClip, volum * PlayerPrefs.GetFloat("sfx"));
    }
    public void SomUI(string clip)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(clip);
        aus.PlayOneShot(audioClip,PlayerPrefs.GetFloat("sfx"));
    }
    //public static void Advanced(string clip,float volum = 1, float pitch = 1)
    //{
    //    AudioClip audioClip = Resources.Load<AudioClip>(clip);

    //    GameObject radio = Resources.Load<GameObject>("AudioPrefab");
    //    Instantiate(radio);
    //    radio.GetComponent<AudioSource>().pitch = pitch;
    //    radio.GetComponent<AudioSource>().PlayOneShot(audioClip, volum );
    //}
}
