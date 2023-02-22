using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    [Header("Stats")]
    public float vida = 500;
    public float maxHp = 500;
    public float baseMaxHp = 500;
    public float movSpeed;
    float baseMovSpeed;
    public float acl;
    float turnSpeed = 7;
    float maxTurn = 2.5f;
    float actTurn;
    public float ancora;
    public float shootPower;
    float actionSpeed;
    public float fire;

    float critChance = 5;
    bool critDmgState = false;
    float dmgChance = 5;
    float dmgFac;
    int dmgFacMult;
    [Header("Resources")]
    public int bulletsRemaining = 15;
    public int wood = 5;
    public TextMeshProUGUI bulletsCount, woodCount;
    [Header("HUD")]
    public GameObject actionStuff;
    public Image actionBar;
    public Image[] cannonUIs;
    public GameObject healingAdvise;
    public GameObject critAdvise;
    public GameObject sinkWarn;
    public GameObject fireWarn;
    public GameObject anchorWarn;
    [Header("Components")]
    [Space]
    public GameObject debris;
    Rigidbody2D rb;
    Transform ship;
    Animator actionAnima;
    public bool busy;
    public Sprite[] healthStates;
    public GameObject[] cannons;
    public Sprite[] sailHealthStates;
    public int[] balas;
    public GameObject cannonBall;
    public int actCannon;
    public Transform cannonRef;
    public Transform sail;
    ParticleSystem dmgPs;
    public ParticleSystem fireParticles;
    [Header("Others")]
    public bool onUI;
    bool anchorRise;
    Manager manager;
    AudioSource aus;
    bool ancorado;
    private void Start()
    {
        Application.targetFrameRate = 60;
        rb = GetComponent<Rigidbody2D>();
        aus = GetComponent<AudioSource>();
        ship = GetComponentInChildren<SpriteRenderer>().transform;
        manager = FindObjectOfType<Manager>();
        dmgPs = GetComponentInChildren<ParticleSystem>();
        bulletsRemaining = 12;
        baseMovSpeed = movSpeed;
        wood = 5;
        actionText = actionStuff.GetComponentInChildren<TextMeshProUGUI>();
        actionAnima = actionStuff.GetComponent<Animator>();
        bulletsCount.text = bulletsRemaining.ToString();
        woodCount.text = wood.ToString();
        anchorWarn.SetActive(true);
        ancorado = true;
        ancora = 2.5f;
        for (int i = 0; i < cannonUIs.Length; i++)
        {
            cannonUIs[i].enabled = false;
        }
    }
    private void Update()
    {
        MovementInput();
        aus.volume = PlayerPrefs.GetFloat("sfx");
        if (ancorado)
        {
            rb.drag = 10;
        }
        else
        {
            rb.drag = 1;
        }
        if (Input.GetMouseButtonDown(0) && !onUI && Time.timeScale != 0)
        {
            if (balas[actCannon] != 0)
            {
                if (actCannon != 0)
                {
                    bool crit = Random.Range(1, 101) < critChance;
                    Quaternion rot = cannons[actCannon].transform.rotation;
                    
                    if (crit)
                    {
                        critChance = 5;
                    }
                    else
                    {
                        critChance += 5;
                    }
                    {
                        GameObject cBall = Instantiate(cannonBall, cannons[actCannon].transform.position, cannons[actCannon].transform.rotation);
                        cBall.GetComponent<Bullet>().crit = crit;
                        Destroy(cBall, 5);
                        cBall.GetComponent<Rigidbody2D>().AddForce((cBall.transform.right * shootPower) /*+ new Vector3(rb.velocity.x, rb.velocity.y, 0)*/, ForceMode2D.Impulse);
                    }
                    {
                        GameObject cBall = Instantiate(cannonBall, cannons[actCannon].transform.position, Quaternion.Euler(rot.x,rot.y,rot.eulerAngles.z + 15));
                        cBall.GetComponent<Bullet>().crit = crit;
                        Destroy(cBall, 5);
                        cBall.GetComponent<Rigidbody2D>().AddForce((cBall.transform.right * shootPower) /*+ new Vector3(rb.velocity.x, rb.velocity.y, 0)*/, ForceMode2D.Impulse);
                    }
                    {
                        GameObject cBall = Instantiate(cannonBall, cannons[actCannon].transform.position, Quaternion.Euler(rot.x, rot.y, rot.eulerAngles.z - 15));
                        cBall.GetComponent<Bullet>().crit = crit;
                        Destroy(cBall, 5);
                        cBall.GetComponent<Rigidbody2D>().AddForce((cBall.transform.right * shootPower) /*+ new Vector3(rb.velocity.x, rb.velocity.y, 0)*/, ForceMode2D.Impulse);
                    }
                }
                else
                {

                    bool crit = Random.Range(1, 101) < critChance;
                    if (crit)
                    {
                        critChance = 5;
                    }
                    else
                    {
                        critChance += 5;
                    }
                    GameObject cBall = Instantiate(cannonBall, cannons[actCannon].transform.position, cannons[actCannon].transform.rotation);
                    cBall.GetComponent<Bullet>().crit = crit;
                    Destroy(cBall, 5);
                    cBall.GetComponent<Rigidbody2D>().AddForce((cBall.transform.right * shootPower)/* + new Vector3(rb.velocity.x, rb.velocity.y, 0)*/, ForceMode2D.Impulse);
                }
                    Som.S("canho");
                    balas[actCannon] = 0;
                    cannonUIs[actCannon].enabled = false;

            }
            else
            {
                Som.S("trig");
            }
        }
        if(fire > 0)
        {
            fire -= Time.deltaTime;
            maxHp -= Time.deltaTime;
            if(vida >= maxHp)
            {
                vida -= Time.deltaTime;
            }
            actionSpeed = 0.6f;
            fireWarn.SetActive(true);
        }
        else if(fire < 0)
        {
            fire = 0;
            fireParticles.Stop();
        }
        else
        {
            actionSpeed = 1;
            fireWarn.SetActive(false);
        }
        if(movSpeed < baseMovSpeed)
        {
            movSpeed += Time.deltaTime * 100;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            actCannon = 0;
            cannons[actCannon].GetComponent<Animator>().SetTrigger("t");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            actCannon = 1;
            cannons[actCannon].GetComponent<Animator>().SetTrigger("t");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            actCannon = 2;
            cannons[actCannon].GetComponent<Animator>().SetTrigger("t");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (busy)
            {
                busy = false;
            }
            else if(balas[actCannon] == 0 && bulletsRemaining > 0)
            {
                StartCoroutine(Load());
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (busy)
            {
                busy = false;
            }else
            if (wood > 0 && vida < baseMaxHp)
            {
                StartCoroutine(Heal());
            }
        }
        if(vida > maxHp)
        {
            vida = maxHp;
        }
        if(vida <= 0)
        {
            maxHp -= Time.deltaTime * 30;
            sinkWarn.SetActive(true);
        }
        if(maxHp <= 0)
        {
            GameObject d =
            Instantiate(debris, transform.position, ship.rotation);
            Destroy(d, 3);
            Destroy(d.GetComponentInChildren<CircleCollider2D>().gameObject, 0.1f);
            Som.S("death");
            manager.Finish(false);
            Destroy(gameObject);
        }
        if (critDmgState)
        {
            
            dmgFac += Time.deltaTime;
            if(dmgFac > 10)
            {
                dmgFac = 0;
                dmgFacMult++;
                TakeDamage(10 * dmgFacMult);
            }
        }
        
        AnchorStuff();
        //cannonRef.position = cannons[0].transform.position;
    }
    private void FixedUpdate()
    {
        Move();
    }

    void AnchorStuff()
    {
        if (anchorRise)
        {
            return;
        }
        if(ancora > 0 && ancora < 2.5f)
        {
            ancora += Time.deltaTime * 1.6f;
            
            if (!ancoraDescendo)
            {
                ancoraDescendo = true;
                aus.pitch = 1;
                aus.Play();
                actionStuff.SetActive(true);
                actionAnima.SetBool("fade",false);
                actionText.text = "<color=yellow>Soltando âncora...";
            }
            else
            {
                if (!busy)
                {
                    actionBar.fillAmount = ancora / 2.5f;
                }
            }
        }
        if (ancora > 2.5f)
        {
            ancoraDescendo = false;
            ancora = 2.5f;
            Som.S("medHE");
            if (!ancorado)
            {
                StartCoroutine(AnchorDrift());
            }
            ancorado = true;
            aus.Stop();

            if (!busy)
            {
                actionAnima.SetBool("fade",true);
                actionText.text = "<color=yellow>Ancorado.";
            }
            
        }


    }
    IEnumerator AnchorDrift()
    {
        if (actTurn > 0.1f || actTurn < -0.1f)
        {
            float turnBoost = 30;
            while (turnBoost > 0)
            {
                ship.Rotate(0, 0, actTurn * Time.deltaTime * turnBoost);
                turnBoost -= Time.deltaTime * 30;
                yield return null;
            }
        }
    }
    IEnumerator Load()
    {
        busy = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        actionStuff.SetActive(true);
        actionAnima.SetBool("fade",false);
        
        actionText.text = "Recarregando...";
        busy = true;
        float timer = 0;
        while(timer < 0.8f)
        {
            timer += Time.deltaTime * actionSpeed;
            actionBar.fillAmount = timer / 0.8f;
            if (!busy)
            {
                actionText.text = "<color=red>Cancelado.";
                actionAnima.SetBool("fade",true);
                busy = false;
                yield break;
            }
            yield return null;
        }
        busy = false;
        actionText.text = "<color=green>Concluído.";
        balas[actCannon] = 1;
        actionAnima.SetBool("fade",true);
        cannonUIs[actCannon].enabled = true;
        Som.S("load");
        bulletsRemaining--;
        bulletsCount.text = bulletsRemaining.ToString();
    }
    IEnumerator Heal()
    {
        busy = false;
        float timer = 0;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        Som.S("saw");
        actionStuff.SetActive(true);
        actionAnima.SetBool("fade",false);
        actionText.text = "<color=white>Consertando...";
        busy = true;
        float delay = 0.5f;
        while(timer < 2)
        {
            timer += Time.deltaTime * actionSpeed;
            actionBar.fillAmount = timer / 2;
            delay -= Time.deltaTime;
            if(delay < 0)
            {
                delay = 0.5f;
                Som.S("dig");
            }
            if (!busy)
            {
                busy = false;
                actionText.text = "<color=red>Cancelado";
                actionAnima.SetBool("fade",true);
                yield break;

            }
          
            yield return null;
        }
        wood--;
        woodCount.text = wood.ToString();
        if(vida <= 0)
        {
            vida += 100;
            sinkWarn.SetActive(false);
        }
        else if(vida >= maxHp)
        {
            maxHp += 50;
            vida += 50;
            if(maxHp > baseMaxHp)
            {
                maxHp = baseMaxHp;
            }
        }
        else
        {
            vida += 200;
            critDmgState = false;
            dmgFac = 0;
            dmgFacMult = 0;
            critAdvise.SetActive(false);
        }
        dmgPs.Play();
        Som.S("dones");
        actionText.text = "<color=green>Concluído.";
        if (vida < maxHp * 0.3f)
        {
            healingAdvise.SetActive(true);
            ship.GetComponent<SpriteRenderer>().sprite = healthStates[2];
            sail.GetComponent<SpriteRenderer>().sprite = sailHealthStates[2];
        }
        else if (vida < maxHp * 0.6f)
        {
            healingAdvise.SetActive(true);
            ship.GetComponent<SpriteRenderer>().sprite = healthStates[1];
            sail.GetComponent<SpriteRenderer>().sprite = sailHealthStates[1];
        }
        else
        {
            healingAdvise.SetActive(false);
            ship.GetComponent<SpriteRenderer>().sprite = healthStates[0];
            sail.GetComponent<SpriteRenderer>().sprite = sailHealthStates[0];
        }
        actionAnima.SetBool("fade",true);
        busy = false;
    }
    void Move()
    {
        if (!ancorado && sail.localScale.y > 0.1f)
        {
            rb.AddForce((movSpeed * sail.localScale.y) * Time.deltaTime * ship.up);
        }
        if (ancora == 2.5f)
        {
            ship.Rotate(0, 0, actTurn * Time.deltaTime * (turnSpeed + 2));
        }
        else
        {
            ship.Rotate(0, 0, actTurn * Time.deltaTime * (turnSpeed + (1 -sail.localScale.y)));
        }
    }
    void MovementInput()
    {
        actTurn += -Input.GetAxisRaw("Horizontal") * Time.deltaTime * 3.5f;
        if(actTurn > 0.3f && Input.GetAxisRaw("Horizontal") == 0)
        {
            actTurn -= Time.deltaTime * 2.5f;
        }else if (actTurn < -0.3f && Input.GetAxisRaw("Horizontal") == 0)
        {
            actTurn += Time.deltaTime * 2.5f;
        }else if(Input.GetAxisRaw("Horizontal") == 0)
        {
            actTurn = 0;
        }
        actTurn = Mathf.Clamp(actTurn, -maxTurn, maxTurn);
        if(Input.GetAxis("Mouse ScrollWheel") < 0 && sail.localScale.y > 0.1f)
        {
            sail.localScale = new Vector2(1, sail.localScale.y-0.05f);
        }else if (Input.GetAxis("Mouse ScrollWheel") > 0 && sail.localScale.y < 1)
        {
            sail.localScale = new Vector2(1, sail.localScale.y + 0.05f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ancorar();
        }
    }
    public void Ancorar()
    {
        
        if(ancora <= 0)
        {
            ancora = 0.01f;
        }else if(!busy)
        {
            StartCoroutine(Anchor());
        }
        else
        {
            busy = false;
        }
    }
    IEnumerator Anchor()
    {
        busy = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        busy = true;
        actionStuff.SetActive(true);
        if (ancoraDescendo)
        {
            ancoraDescendo = false;
        }
        else
        {
        actionAnima.SetBool("fade",false);

        }
        anchorRise = true;
        aus.Play();
        aus.pitch = 0.5f;
        float fill = 2.5f - ancora;
        actionBar.fillAmount = fill/2.5f;
        
        actionText.text = "Levantando âncora...";
        while(ancora > 0)
        {
            ancora -= Time.deltaTime * 0.8f * actionSpeed;
            fill = 2.5f - ancora;
            actionBar.fillAmount = fill /2.5f;
            if (!busy)
            {
                actionAnima.SetBool("fade",true);
                actionText.text = "<color=red>Cancelado.";
                aus.Stop();
                busy = false;
                ancoraDescendo = true;
                anchorRise = false;
                yield break;
            }
            yield return null;
        }
        
        busy = false;
        ancorado = false;
        anchorRise = false;
        aus.Stop();
        anchorWarn.SetActive(false);
        actionText.text = "<color=green>Concluído.";
        actionAnima.SetBool("fade",true);
        Som.S("timaoReset");
    }
    bool ancoraSubindo, ancoraDescendo;
    TextMeshProUGUI actionText;
    bool grounded;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6 && movSpeed > 0)
        {
            if (movSpeed > 0)
            {

                movSpeed -= Time.deltaTime * 300;
                grounded = true;
                vida -= Time.deltaTime * 140;
            }
            else
            {
                if (vida <= 0)
                {
                    maxHp -= Time.deltaTime * 100;

                }
                else
                {
                    vida -= Time.deltaTime * 500;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            movSpeed = baseMovSpeed;
            TakeDamage(1);
            grounded = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            TakeDamage(rb.velocity.magnitude * 100,true);
        }
        if (collision.gameObject.layer == 6)
        {
            
            TakeDamage(1);
            grounded = true;
            StartCoroutine(LandDmg());
        }
    }
    IEnumerator LandDmg()
    {
        yield return new WaitForSeconds(1);
        if (grounded)
        {
            TakeDamage(1);
            StartCoroutine(LandDmg());
        }
    }
    public void TakeDamage(float dmg,bool guarantee = false)
    {
        bool criticalHit = Random.Range(1, 101) < dmgChance;
        if (guarantee)
        {
            criticalHit = true;
        }
        if (criticalHit)
        {
            Som.S("crit");
            critDmgState = true;
            critAdvise.SetActive(true);
            dmgChance = 5;
            dmg *= 2;
            vida -= dmg;
            if(vida <= 0)
            {
                maxHp -= Mathf.Abs(vida * 0.5f);
                Som.S("aniquilado");
                vida = 0;
            }
        }
        else
        {
            dmgChance += 5;
            vida -= dmg;
            if (vida <= 0)
            {
                maxHp -= Mathf.Abs(vida * 0.5f);
                Som.S("aniquilado");
                vida = 0;
            }
        }
        if(vida < 0)
        {
            Som.S("aniquilado");
            vida = 0;
        }
        dmgPs.Play();
        if (vida < maxHp * 0.3f)
        {
            healingAdvise.SetActive(true);
            ship.GetComponent<SpriteRenderer>().sprite = healthStates[2];
            sail.GetComponent<SpriteRenderer>().sprite = sailHealthStates[2];
        }else if (vida < maxHp * 0.6f)
        {
            healingAdvise.SetActive(true);
            ship.GetComponent<SpriteRenderer>().sprite = healthStates[1];
            sail.GetComponent<SpriteRenderer>().sprite = sailHealthStates[1];
        }
        else
        {
            healingAdvise.SetActive(false);
            ship.GetComponent<SpriteRenderer>().sprite = healthStates[0];
            sail.GetComponent<SpriteRenderer>().sprite = sailHealthStates[0];
        }
    }
}
