using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Invader : MonoBehaviour
{
    public GameObject fireBurst;
    public GameObject pop;

    public void Invade(Ship ship)
    {
        Som.S("invadido");
        GameObject go =
        Instantiate(pop, ship.transform.position, Quaternion.identity);
        Destroy(go, 5);//textinho que avisa o jogador sobre a invasão
        StartCoroutine(Invasion(ship,go.GetComponentInChildren<TextMeshProUGUI>()));
    }
    public IEnumerator Invasion(Ship ship,TextMeshProUGUI tmpug)
    {
        int rando = Random.Range(1, 6);

        for (int i = 0; i < ship.balas.Length; i++)
        {
            ship.balas[i] = 0; // sempre que o jogador for invadido, todos canhoes carregados serão descarregados
            ship.cannonUIs[i].enabled = false;
        }
        switch (rando)
        {
            case 1: //Tira de 2 a 6 balas do jogador
                StartCoroutine(Steal(ship,tmpug,false));
                break;

            case 2: //Tira de 2 a 6 tabuas do jogador
                StartCoroutine(Steal(ship, tmpug, true));
                break;
            case 3://solta a ancora

                if (ship.ancora <= 0)
                {
                    tmpug.text = "ÂNCORA SOLTA!!!";
                    ship.ancora = 0.1f;
                }
                else
                {
                    StartCoroutine(Invasion(ship, tmpug));
                    yield break;
                }
                break;
            case 4://constantemente fica causando um pequeno dano e cancelando a ação atual do jogador por um curto periodo de tempo
                StartCoroutine(Stall(ship, tmpug));
                break;
            case 5://aplica fogo no jogador
                tmpug.text = "EM CHAMAS!!!";
                Som.S("fire");
                ship.fire += Random.Range(6f, 10f);
                ship.fireParticles.Play();
                GameObject particles =
                Instantiate(fireBurst, ship.transform.position, Quaternion.identity);
                Destroy(particles, 5);
                break;
        }
        
    }

    IEnumerator Steal(Ship ship, TextMeshProUGUI tmpug, bool wood)
    {
        if (!wood)
        {
            if (ship.bulletsRemaining > 0)
            {
                tmpug.text = "ROUBADO!!!";
                int reduction = Random.Range(2, 6);
                while (reduction > 0)
                {
                    if (ship.bulletsRemaining <= 0)
                    {
                        ship.bulletsRemaining = 0;
                        ship.bulletsCount.text = ship.bulletsRemaining.ToString();
                        reduction = 0;
                    }
                    else
                    {
                        Som.S("pick");
                        reduction--;
                        ship.bulletsRemaining--;
                        ship.bulletsCount.text = ship.bulletsRemaining.ToString();
                    }
                    yield return new WaitForSeconds(0.3f);
                    yield return null;
                }

            }
            else
            {
                StartCoroutine(Invasion(ship, tmpug));
                yield break;
            }
        }
        else
        {
            if (ship.wood > 0)
            {
                tmpug.text = "ROUBADO!!!";
                int reduction = Random.Range(2, 6);
                while (reduction > 0)
                {
                    if (ship.wood <= 0)
                    {
                        ship.wood = 0;
                        ship.woodCount.text = ship.wood.ToString();
                        reduction = 0;
                    }
                    else
                    {
                        Som.S("pick");
                        reduction--;
                        ship.wood--;
                        ship.woodCount.text = ship.wood.ToString();
                    }
                    yield return new WaitForSeconds(0.3f);
                    yield return null;
                }

            }
            else
            {
                StartCoroutine(Invasion(ship, tmpug));
                yield break;
            }
        }
    }
    IEnumerator Stall(Ship ship, TextMeshProUGUI tmpug)
    {
        tmpug.text = "INVADIDO!!!";
        bool again = true;
        float chance = 100;
        yield return new WaitForSeconds(0.6f);
        while (again)
        {
            float lNumber = Random.Range(1f, 100f);
            if (lNumber < chance)
            {
                chance *= 0.85f;
                ship.TakeDamage(5);
                ship.busy = false;
                Som.S("bleed");
              
                yield return new WaitForSeconds(0.6f);
                yield return null;
            }
            else
            {
                again = false;
            }
        }
    }
}
