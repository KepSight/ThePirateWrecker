using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Menu : MonoBehaviour
{
    public Slider sfx, music;
    public TextMeshProUGUI sfxTxt, musicTxt;
    public Slider duration, spawnRate;
    public TextMeshProUGUI spawnRateTxt;
    public TextMeshProUGUI durationTxt;
    AudioSource aus;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("spawnRate"))
        {
            PlayerPrefs.SetFloat("spawnRate", 10);
        }
        if (!PlayerPrefs.HasKey("duration"))
        {
            PlayerPrefs.SetFloat("duration", 180);
        }
        if (!PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetFloat("sfx", 0.5f);
        }
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetFloat("music", 0.25f);
        }
        aus = GetComponent<AudioSource>();
        Time.timeScale = 1;
    }
    private void Update()
    {
        aus.volume = PlayerPrefs.GetFloat("music");
    }
    public void Play()
    {
        SceneManager.LoadScene("PlayScene");
    }
    public void AOff(GameObject o)
    {
        o.SetActive(false);
    }
    public void AOn(GameObject o)
    {
        o.SetActive(true);
    }
    public void SetSpawnRate(float rate)
    {
        PlayerPrefs.SetFloat("spawnRate", rate);
        spawnRateTxt.text = $"{rate:F0}";
    }
    public void SetDuration(float value)
    {
        PlayerPrefs.SetFloat("duration", value);
        durationTxt.text = $"{value:F0}";
    }
    public void SetSfx(float value)
    {
        PlayerPrefs.SetFloat("sfx", value);
        sfxTxt.text = $"{value * 100:F0}";
    }
    public void SetMusic(float value)
    {
        PlayerPrefs.SetFloat("music", value);
        musicTxt.text = $"{value * 100:F0}";
    }
    public void SetSliders()
    {
        sfx.value = PlayerPrefs.GetFloat("sfx");
        music.value = PlayerPrefs.GetFloat("music");
        spawnRate.value = PlayerPrefs.GetFloat("spawnRate");
        duration.value = PlayerPrefs.GetFloat("duration");
        spawnRateTxt.text = $"{spawnRate.value:F0}";
        durationTxt.text = $"{duration.value:F0}";
        sfxTxt.text = $"{sfx.value * 100:F0}";
        musicTxt.text = $"{music.value * 100:F0}";
    }
}
