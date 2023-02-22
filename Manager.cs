using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public TextMeshProUGUI pointsTxt;
    public TextMeshProUGUI tempoTxt;
    public float points;
        float s, ms, m, t;
    public int bulletPrice, woodPrice;
    Ship ship;
    [Space]
    public GameObject menu;
    public Slider sfx, music;
    public TextMeshProUGUI sfxTxt, musicTxt;
    float pointsPrSecond;
    bool ended;
    public GameObject victory;
    AudioSource musicAus;
    float buyCD;
    public Button[] buys;
    private void Awake()
    {
    }
    private void Start()
    {
        ship = FindObjectOfType<Ship>();
        Time.timeScale = 0.01f;
        musicAus = GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetFloat("music", 0.25f);
        }
        musicAus.volume = PlayerPrefs.GetFloat("music");
        Time.timeScale = 1;
    }
    public void OnUI(bool on)
    {
        ship.onUI = on;
    }
    private void Update()
    {
        if (ended)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.X) && buyCD <=0)
        {
            BuyStuff(true);
        }
        if (Input.GetKeyDown(KeyCode.Z) && buyCD <= 0)
        {
            BuyStuff(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                menu.SetActive(false);
                Som.S("back");
                musicAus.Play();
            }
            else
            {
                SetStuff();
                Som.S("pause");
                menu.SetActive(true);
                musicAus.Pause();
                Time.timeScale = 0;
            }
        }
        if(buyCD > 0)
        {
            buyCD -= Time.deltaTime;
        }
        if(buyCD < 0)
        {
            buyCD = 0;
            buys[0].interactable = true;
            buys[1].interactable = true;
        }
        pointsPrSecond += Time.deltaTime;
        if(pointsPrSecond> 1)
        {
            pointsPrSecond -= 1;
            points += 5;
            pointsTxt.text = $"Pontuação: {points:F0}";
        }
        t += Time.deltaTime;
        if(t>= PlayerPrefs.GetFloat("duration"))
        {
            Finish(true);
        }
        ms = (int)((t - (int)t) * 100);
        s = (int)(t % 60);
        m = (int)(t / 60 % 60);
        tempoTxt.text = string.Format("{0:00}:{1:00}:{2:00}", m, s, ms);
    }
    public void Finish(bool win)
    {
        ended = true;victory.SetActive(true);StartCoroutine(Slowslow());
        if (win)
        {
            victory.GetComponentInChildren<TextMeshProUGUI>().text = $"VITÓRIA\n{points:F0} Pontos";
        }
        else
        {

            points *= 0.3f;
            pointsTxt.text = $"Pontuação: {points:F0}";
            victory.GetComponentInChildren<TextMeshProUGUI>().text = $"DERROTA\n<color=red>{points:F0} Pontos";
        }
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    IEnumerator Slowslow()
    {
        float timer = 1;
        yield return new WaitForSeconds(1f);
        while(timer > 0.1f)
        {
            timer -= Time.unscaledDeltaTime;
            Time.timeScale = timer;
            yield return null;
        }
        Time.timeScale = 0;
    }

    public void AddPoints(int pts)
    {
        pointsTxt.gameObject.SetActive(false);
        pointsTxt.gameObject.SetActive(true);
        
        points += pts;
        pointsTxt.text = $"Pontuação: {points:F0}";
    }
    public void BuyStuff(bool bullet)
    {
        if(bullet)
        {
            if(points > bulletPrice)
            {
                points -= bulletPrice;
                pointsTxt.text = $"Pontuação: {points:F0}";
                ship.bulletsRemaining++;
                ship.bulletsCount.text = ship.bulletsRemaining.ToString();
                buyCD = 1;
                buys[0].interactable = false;
                buys[1].interactable = false;
                Som.S("coin");
            }
        }
        else
        {
            if (points > woodPrice)
            {
                points -= woodPrice;
                pointsTxt.text = $"Pontuação: {points:F0}";
                ship.wood++;
                ship.woodCount.text = ship.wood.ToString();
                buys[0].interactable = false;
                buys[1].interactable = false;
                buyCD = 1;
                Som.S("coin");
            }
        }
    }

    public void Close()
    {
        Time.timeScale = 1;
        musicAus.Play();
        menu.SetActive(false);
    }
    public void ADisable(GameObject g)
    {
        g.SetActive(false);
    }
    public void AEnable(GameObject g)
    {
        g.SetActive(true);
    }

    public void SetStuff()
    {
        if (!PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetFloat("sfx", 0.5f);
        }
        if (!PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetFloat("music", 0.5f);
        }
        sfx.value = PlayerPrefs.GetFloat("sfx");
        music.value = PlayerPrefs.GetFloat("music");
        sfxTxt.text = $"{sfx.value * 100:F0}";
        musicTxt.text = $"{music.value * 100:F0}";
    }
    public void SetSfx(float value)
    {
        PlayerPrefs.SetFloat("sfx", value);
        sfxTxt.text = $"{value * 100:F0}";
    }
    public void SetMusic(float value)
    {
        PlayerPrefs.SetFloat("music", value);
        musicTxt.text = $"{value *100:F0}";
        musicAus.volume = value;
    }

}
