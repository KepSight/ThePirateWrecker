using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value, wood, bullets;
    bool collected = false;
    public GameObject popText;

    private void Awake()
    {

        wood = Random.Range(0, 4);
        bullets = Random.Range(1, 6);
        //StartCoroutine(Delay());
    }
    //IEnumerator Delay()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    collected = false;
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)
        {
            return;
        }
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name.Contains("cnBlAlly"))
        {
            GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
            GetComponentInChildren<ParticleSystem>().Play();
            GameObject txt=
            Instantiate(popText, transform.position, Quaternion.identity);
            txt.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=orange>+{value} Pontos";
            if(wood > 1)
            {
                txt.GetComponentInChildren<TextMeshProUGUI>().text += $"\n<color=yellow>+{wood} Tábuas";
            }else if (wood > 0)
            {
                txt.GetComponentInChildren<TextMeshProUGUI>().text += $"\n<color=yellow>+{wood} Tábua";
            }
            else
            if (bullets > 1)
            {
                txt.GetComponentInChildren<TextMeshProUGUI>().text += $"\n<color=white>+{bullets} Balas";
            }else if (bullets > 0)
            {
                txt.GetComponentInChildren<TextMeshProUGUI>().text += $"\n<color=white>+{bullets} Bala";
            }
            Som.S("grab");
            Ship ship =
            FindObjectOfType<Ship>();
            ship.wood += wood;
            ship.woodCount.text = ship.wood.ToString();
            ship.bulletsRemaining += bullets;
            ship.bulletsCount.text = ship.bulletsRemaining.ToString();
            FindObjectOfType<Manager>().AddPoints(value);
            collected = true;
            Destroy(txt, 2);
            Destroy(gameObject, 0.3f);
        }
    }
}
