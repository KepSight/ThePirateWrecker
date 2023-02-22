using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    public int power;
    public bool ally;
    public ParticleSystem ps;
    bool destroy;
    public bool crit;
    public float guidance;
    Transform target;
    Rigidbody2D rb;
    float speed;
    public float lightness = 1;
    public float angleOffset;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity *= lightness;
        if(angleOffset != 0)
        {
            transform.Rotate(0,0,angleOffset);
        }
        if (ally)
        {
            float closest = Mathf.Infinity;
            GameObject closeOne;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach(GameObject enemy in enemies)
            {
                float actDistance = (enemy.transform.position - transform.position).sqrMagnitude;
                if(actDistance < closest)
                {
                    closest = actDistance;
                    closeOne = enemy;
                    target = closeOne.transform;
                }
            }
        }
        else
        {
            target = FindObjectOfType<Ship>().transform;
        }
        speed = rb.velocity.magnitude;
        StartCoroutine(Effect());
    }
    IEnumerator Effect()
    {
        yield return new WaitForSeconds(4.5f);
        destroy = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (GetComponentInChildren<TrailRenderer>())
        {
            GetComponentInChildren<TrailRenderer>().emitting = false;
        }
        ps.Play();
    }
   
    private void Update()
    {
        if (guidance > 0 && target != null)
        {
            Vector2 direction = (Vector2)target.position - rb.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotateAmount * guidance;

            rb.AddForce(transform.up * speed);


        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destroy)
        {
            return;
        }
        if (ally)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if(collision.gameObject.GetComponent<Inimigo>())
                {   
                    if(collision.gameObject.GetComponent<Inimigo>().hp <= 0)
                    {
                        return;
                    }
                    if (crit)
                    {
                        collision.gameObject.GetComponent<Inimigo>().TakeDmg(power*2);
                    }
                    else
                    {
                        collision.gameObject.GetComponent<Inimigo>().TakeDmg(power);
                    }

                }
                else if (collision.gameObject.GetComponent<Boss>())
                {
                    if (collision.gameObject.GetComponent<Boss>().hp <= 0)
                    {
                        return;
                    }
                    collision.gameObject.GetComponent<Boss>().TakeDmg(power);
                }
                    GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                destroy = true;
                FindObjectOfType<Manager>().AddPoints(power);
                Destroy(gameObject, 1);
                Som.S("bigHE");
                Som.S("mark");
                ps.Play();
                if (GetComponentInChildren<TrailRenderer>())
                {
                    GetComponentInChildren<TrailRenderer>().emitting = false;
                }
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<Ship>().TakeDamage(power,crit);
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                destroy = true;
                Destroy(gameObject, 1);
                Som.S("bigHE");
                ps.Play();
                if (GetComponentInChildren<TrailRenderer>())
                {
                    GetComponentInChildren<TrailRenderer>().emitting = false;
                }
            }
        }
    }
}
