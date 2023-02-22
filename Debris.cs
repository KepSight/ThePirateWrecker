using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    Rigidbody2D rb;
    public bool sink;
    private void Start()
    {
        if(GetComponent<Rigidbody2D>() != null)
        {

        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = Random.Range(30, 200);
        }
        StartCoroutine(Sink());
    }
    IEnumerator Sink()
    {
        float timer = 3;
        if (sink)
        {
            SpriteRenderer sr =
            GetComponent<SpriteRenderer>();
            while(timer > 0)
            {
                timer -= Time.deltaTime;
                transform.localScale = Vector3.one * (timer / 3);
                sr.color = Vector4.one * (timer / 3);
                yield return null;
            }
        }
        else
        {
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer < 1)
                {
                    transform.localScale = Vector3.one * timer;
                }
                yield return null;
            }
        }
        Destroy(gameObject);
    }
}
