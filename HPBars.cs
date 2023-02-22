using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBars : MonoBehaviour
{
    public Image barra;
    public Image barraSink;
    public Ship alvo;

    private void Update()
    {
        barraSink.fillAmount = alvo.maxHp / alvo.baseMaxHp;
        barra.fillAmount = alvo.vida / 500;
    }
}
