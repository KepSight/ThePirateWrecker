using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int value;
    public float spd;
    public float range;
    public float shotRange;
    public float turnSpd;
    public float hp;
    public float maxHp = 250;
    public float shotPower = 30;
    float actSpd;
    float cd;
    float minionCd;
    public float angleOffset;
    public float shotOffset;
    bool inRange;
    [Header("Components")]
    public GameObject[] minions;
    public GameObject[] bullets;
    public Image hpBar;
    public Sprite[] healthStates;
    public GameObject debris, treasure;
    public Transform cannon, cannonR;
    public Transform fixo,fixoR;
    public GameObject cannonB;
    public ParticleSystem dmgParticles;
    public ParticleSystem phantomParticles;
    public Transform auxilio;
    Transform hpBarParent;
    Rigidbody2D rb;
    Transform target;
    [Header("Others")]
    Quaternion startRotation;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<Ship>().transform;
        hpBarParent = hpBar.GetComponentInParent<Canvas>().transform;
        startRotation = Quaternion.identity;
        hp = maxHp;
        minionCd = Random.Range(4, 9);
        cannon = fixo;
        cannonR = fixoR;
        StartCoroutine(Minions());
    }
    IEnumerator Minions()
    {

        while(minionCd > 0)
        {
            minionCd -= Time.deltaTime;
            yield return null;
        }
        int minino = Random.Range(0, minions.Length);
        Instantiate(minions[minino],transform.position,transform.rotation);
        minionCd = Random.Range(12, 26);
        StartCoroutine(Minions());
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }
        Movement();
        if (Vector2.Distance(transform.position, target.position) < range)
        {
            inRange = true;
            Shoot();
        }else if(Vector2.Distance(transform.position, target.position) < shotRange)
        {
            inRange=false;
            Shoot();
        }
        else
        {
            inRange = false;
        }
        if (cd > 0)
        {
            cd -= Time.deltaTime;
        }
        hpBar.fillAmount = hp / maxHp;
        if (hp <= 0)
        {
            GameObject d =
            Instantiate(debris, transform.position, transform.rotation);
            Destroy(d.GetComponentInChildren<CircleCollider2D>().gameObject, 0.1f);
            Destroy(d, 3);
            if (value != 0)
            {
                GameObject t = Instantiate(treasure, transform.position, Quaternion.identity);
                value += (Random.Range(1, 4)) * 100;
                t.GetComponent<Treasure>().value = value;
            }

            Som.S("kill");
            Destroy(gameObject);
        }
    }
    void Movement()
    {
        
        if (inRange)
        {

            actSpd = Mathf.Lerp(0.01f, spd, Vector2.Distance(transform.position, target.position) / range);
            rb.drag = Mathf.Lerp(1, 2, Vector2.Distance(transform.position, target.position) / range);
            

        }
        else
        {
            actSpd = spd * (Vector2.Distance(transform.position, target.position) * 0.1f);
            rb.drag = 1;
        }
    }
    void Shoot()
    {
        cannonB = bullets[Random.Range(0, bullets.Length)];
        if (cd <= 0)
        {
            {
                GameObject bala =
                Instantiate(cannonB, cannon.position, cannon.rotation);
                Destroy(bala, 5);
                bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * shotPower, ForceMode2D.Impulse);
                Som.S("tiro");
                cd = 2.5f;
            }
            {
                GameObject bala =
                Instantiate(cannonB, cannon.position, Quaternion.Euler(cannon.rotation.x, cannon.rotation.y, cannon.rotation.eulerAngles.z + shotOffset));
                Destroy(bala, 5);
                bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * (shotPower * 0.75f), ForceMode2D.Impulse);

            }
            {
                GameObject bala =
                Instantiate(cannonB, cannon.position, Quaternion.Euler(cannon.rotation.x, cannon.rotation.y, cannon.rotation.eulerAngles.z - shotOffset));
                Destroy(bala, 5);
                bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * (shotPower * 0.75f), ForceMode2D.Impulse);

            }


            {
                GameObject bala =
                Instantiate(cannonB, cannonR.position, cannonR.rotation);
                Destroy(bala, 5);
                bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * shotPower, ForceMode2D.Impulse);

            }
            {
                GameObject bala =
                Instantiate(cannonB, cannonR.position, Quaternion.Euler(cannonR.rotation.x, cannonR.rotation.y, cannonR.rotation.eulerAngles.z + shotOffset));
                Destroy(bala, 5);
                bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * (shotPower * 0.75f), ForceMode2D.Impulse);

            }
            {
                GameObject bala =
                Instantiate(cannonB, cannonR.position, Quaternion.Euler(cannonR.rotation.x, cannonR.rotation.y, cannonR.rotation.eulerAngles.z - shotOffset));
                Destroy(bala, 5);
                bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * (shotPower * 0.75f), ForceMode2D.Impulse);

            }
        }
    }

    void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        if (!inRange)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * turnSpd;
            rb.AddForce(transform.up * actSpd);
        }
        else
        {
            Vector3 diff = target.position - transform.position;
            float rZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            auxilio.rotation = Quaternion.Euler(0, 0, rZ + angleOffset);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, auxilio.rotation, Time.deltaTime * turnSpd);

            rb.AddForce(transform.up * (actSpd * 0.5f));
            rb.angularVelocity = 0;
        }


    }
    private void LateUpdate()
    {
        hpBarParent.rotation = startRotation;
    }
   
    public void TakeDmg(float dmg)
    {
        hp -= dmg;
        dmgParticles.Play();
        minionCd -= 1;
        if (hp < maxHp * 0.3f)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = healthStates[2];

        }
        else if (hp < maxHp * 0.6f)
        {
            GetComponentInChildren<SpriteRenderer>().sprite = healthStates[1];

        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().sprite = healthStates[0];

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            phantomParticles.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            phantomParticles.Stop();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Ship>().TakeDamage(hp*3);
            TakeDmg(150);
            Vector2 kb = Vector2.Lerp(collision.transform.position, transform.position, 0.5f).normalized;
            rb.AddForce(kb * 10);
            collision.rigidbody.AddForce(kb * -10);

        }
        if(collision.gameObject.layer == 7)
        {
            transform.Rotate(0, 0, Random.Range(-30f,30f));
            TakeDmg(hp * 0.1f);
        }

    }
}
