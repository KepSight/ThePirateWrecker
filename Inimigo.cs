using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inimigo : MonoBehaviour
{
    [Header("Stats")]
    public int value;
    public float spd;
    float actSpd;
    public bool melee;
    public float range;
    public float turnSpd;
    public float hp;
    public float maxHp = 250;
    public float shotPower = 30;
    float cd;
    public float shootCd = 2.5f;
    bool sinking = false;
    float drag;
    bool far;
    [Header("Components")]
    public ParticleSystem dmgParticles;
    public Image hpBar;
    public Sprite[] healthStates;
    Rigidbody2D rb;
    Transform hpBarParent;
    public Transform cannon;
    public Transform fixo;
    Quaternion assist, auxA, auxB;
    Transform target;
    float fac;

    [Header("GameObjects")]
    public GameObject cannonB;
    public GameObject debris, treasure;
    Quaternion startRotation;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<Ship>().transform;
        hpBarParent = hpBar.GetComponentInParent<Canvas>().transform;
        cd = shootCd;
        startRotation = Quaternion.identity;
        drag = rb.drag;
    }

    private void Update()
    {

        if (target == null || sinking)
        {
            return;
        }
        if (Vector2.Distance(transform.position, target.position) < range)
        {

            if (!melee)
            {

                actSpd = Mathf.Lerp(0.01f, spd, Vector2.Distance(transform.position, target.position) / range);
                if (landTime <= 0)
                {
                    rb.drag = Mathf.Lerp(1, 2, Vector2.Distance(transform.position, target.position) / range);
                }
                ReadyToShoot();
            }
            else
            {
                if (far)
                {
                    actSpd = spd;
                    far = false;
                }
            }
        }
        else
        {
            actSpd = spd * (Vector2.Distance(transform.position, target.position) * 0.1f);
            far = true;

            if (landTime <= 0)
            {
                rb.drag = 1;
            }
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
            Destroy(d, 3);
            Destroy(d.GetComponentInChildren<CircleCollider2D>().gameObject, 0.1f);

            if (value != 0)
            {
                GameObject t = Instantiate(treasure, transform.position, Quaternion.identity);
                value += (Random.Range(1, 4)) * 100;
                t.GetComponent<Treasure>().value = value;
            }

            Som.S("kill");
            hpBarParent.gameObject.SetActive(false);
            if (GetComponent<PolygonCollider2D>())
            {
                GetComponent<PolygonCollider2D>().enabled = false;
            }
            if (GetComponent<CircleCollider2D>())
            {
                GetComponent<CircleCollider2D>().enabled = false;
            }
            if (GetComponent<SpriteRenderer>())
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
            if (GetComponentInChildren<SpriteRenderer>())
            {
                for (int i = 0; i < GetComponentsInChildren<SpriteRenderer>().Length; i++)
                {
                    GetComponentsInChildren<SpriteRenderer>()[i].enabled = false;
                }
            }
            sinking = true;
            Destroy(gameObject, 5);
        }
    }
    void FixedUpdate()
    {
        if (target == null || sinking)
        {
            return;
        }
        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * turnSpd;
        rb.AddForce(transform.up * actSpd);
    }
    private void LateUpdate()
    {
        hpBarParent.rotation = startRotation;


    }
    void ReadyToShoot()
    {

        Vector3 diff = target.position - transform.position;
        float rZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        assist = Quaternion.Euler(0, 0, rZ);
        bool turnRight = Mathf.DeltaAngle(cannon.rotation.eulerAngles.z, assist.eulerAngles.z) > 0;
        if (turnRight)
        {
            fac += Time.deltaTime * spd;
        }
        else
        {
            fac -= Time.deltaTime * spd;
        }
        fac = Mathf.Clamp(fac, 0, 1);
        auxA = Quaternion.Euler(0, 0, fixo.rotation.eulerAngles.z - 30);
        auxB = Quaternion.Euler(0, 0, fixo.rotation.eulerAngles.z + 30);
        cannon.rotation = Quaternion.Lerp(auxA, auxB, fac);

        if (Physics2D.Raycast(cannon.position, cannon.up, range))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if (cd <= 0)
        {
            GameObject bala =
            Instantiate(cannonB, cannon.position, cannon.rotation);
            Destroy(bala, 5);
            bala.GetComponent<Rigidbody2D>().AddForce(bala.transform.right * shotPower, ForceMode2D.Impulse);
            Som.S("tiro");
            cd = shootCd;
        }
    }
    public void TakeDmg(float dmg)
    {
        dmg *= Random.Range(0.9f, 1.1f);
        hp -= dmg;
        dmgParticles.Play();
        cd = shootCd;
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
    float landTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            rb.drag = drag * 5;
            landTime += Time.deltaTime;
            hp -= Time.deltaTime * 180;
            if (landTime > 0.5f)
            {
                TakeDmg(10);
                landTime = 0.1f;
            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        landTime = 0;
        rb.drag = drag;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sinking)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (melee)
            {
                if (GetComponent<Invader>())
                {
                    GetComponent<Invader>().Invade(target.GetComponent<Ship>());
                    collision.gameObject.GetComponent<Ship>().TakeDamage(hp * 0.3f);
                    TakeDmg(maxHp*10);
                }
                else
                {
                    collision.gameObject.GetComponent<Ship>().TakeDamage(hp * 2);
                    TakeDmg(maxHp*10);
                }
            }
            else
            {
                collision.gameObject.GetComponent<Ship>().TakeDamage(hp * 0.3f);
                TakeDmg(maxHp * 0.3f);
            }
            Vector2 kb = Vector2.Lerp(collision.transform.position, transform.position, 0.5f).normalized;
            rb.AddForce(kb * 10);
            collision.rigidbody.AddForce(kb * -10);
            
        }
        if (collision.gameObject.layer == 7)
        {
            transform.Rotate(0, 0, Random.Range(-30f, 30f));
            TakeDmg(maxHp * 0.3f);
        }

    }

}
