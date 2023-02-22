using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
   Ship ship;
    public Vector2 limits;
    SpriteRenderer fogSprite;
    float distance;
    float timer;
    public float velocity;
    Vector2 direction;
    public Transform wind;
    float windForce;
    float windTargetForce;
    float windTargetRotation;
    public float windRotationSpeed;
    ParticleSystem ps;
    ParticleSystem.MainModule mainModule;
    ParticleSystem.EmissionModule emissionModule;
    AreaEffector2D ae2d;
    private void Start()
    {
        ship = FindObjectOfType<Ship>();
        ae2d = wind.gameObject.GetComponentInChildren<AreaEffector2D>();
        ps = wind.gameObject.GetComponentInChildren<ParticleSystem>();
        mainModule = ps.main;
        emissionModule = ps.emission;
        fogSprite = GetComponent<SpriteRenderer>();
        wind.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 359f));
        windTargetRotation = Random.Range(-180f, 180f);
        windTargetForce = Random.Range(0.2f, 2f);
        direction.x = Random.Range(0.5f, 1);
        direction.y = Random.Range(0.5f, 1);

        bool x = Random.Range(1, 101) < 50;
        bool y = Random.Range(1, 101) < 50;

        if (x)
        {
            direction.x *= -1;
        }
        if (y)
        {
            direction.y *= -1;
        }
        StartCoroutine(ChangeDirection(Random.Range(10f, 30f)));
    }
    
    private void Update()
    {
        if (ship == null)
        {
            return;
        }
        wind.transform.position = ship.transform.position;
        if(windTargetRotation > 0)
        {
            wind.transform.Rotate(0, 0, Mathf.Lerp(0,windTargetRotation,Time.deltaTime * windRotationSpeed));
            windTargetRotation -= Time.deltaTime;
            if(windTargetRotation < 0)
            {
                windTargetRotation = 0;
            }
        }else if(windTargetRotation < 0)
        {
            wind.transform.Rotate(0, 0, Mathf.Lerp(0, windTargetRotation, Time.deltaTime * windRotationSpeed));
            windTargetRotation += Time.deltaTime;
            if (windTargetRotation > 0)
            {
                windTargetRotation = 0;
            }
        }
       if(windForce < windTargetForce)
        {
            windForce += Time.deltaTime * 0.3f;

        }else if(windForce> windTargetForce)
        {
            windForce -= Time.deltaTime * 0.3f;
        }
        ae2d.forceMagnitude = windForce;
        
        
        mainModule.simulationSpeed= windForce/2;
        emissionModule.rateOverTime = windForce * 5;
        
        distance = Vector2.Distance(transform.position, ship.transform.position);   
        if(distance > limits.x)
        {
           fogSprite.color = new Color(1,1,1,(distance - limits.x)/ (limits.y - limits.x));

            if (distance > limits.y)
            {
                timer += Time.deltaTime * Mathf.Lerp(30, 100, (distance - limits.y) / (limits.y * 2));
            }
            else
            {
                timer += Time.deltaTime * Mathf.Lerp(1, 30, (distance - limits.x) / (limits.y - limits.x));
            }
            if (timer > 10)
            {

                ship.TakeDamage(15, true);
                timer = 0;
            }
        }
        else { fogSprite.color = Color.clear; }
    }
    IEnumerator ChangeDirection(float delay)
    {
        yield return new WaitForSeconds(delay);
        windTargetRotation = Random.Range(-180f, 180f);
        windTargetForce = Random.Range(0.2f, 2f);
        direction.x = Random.Range(0.5f, 1);
        direction.y = Random.Range(0.5f, 1);

        bool x = Random.Range(1, 101) < 50;
        bool y = Random.Range(1, 101) < 50;

        if (x)
        {
            direction.x *= -1;
        }
        if (y)
        {
            direction.y *= -1;
        }
        StartCoroutine(ChangeDirection(Random.Range(10f, 30f)));
    }
    private void FixedUpdate()
    {
        transform.Translate(Time.deltaTime * velocity * direction);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * limits.x));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position +Vector3.up, transform.position+Vector3.up + (Vector3.right * limits.y));
    }
}
