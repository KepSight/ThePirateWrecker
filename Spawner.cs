using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float delay;
    public Vector2 distance;
    float cd;
    public GameObject[] enemies;
    Transform ship;
    public GameObject boss;

    private void Start()
    {
        delay = PlayerPrefs.GetFloat("spawnRate");
        ship = FindObjectOfType<Ship>().transform;
        StartCoroutine(Spawnar(delay));
        StartCoroutine(Spawnar(delay));
        StartCoroutine(Boss(PlayerPrefs.GetFloat("duration") * 0.5f));
    }
    IEnumerator Spawnar(float interval)
    {
        yield return new WaitForSeconds(interval);
        int enemyToSpawn = Random.Range(0, enemies.Length);
        Vector3 offset = new Vector2(Random.Range(distance.x,distance.y), Random.Range(distance.x, distance.y));
        if (ship == null)
        {
            yield break;
        }
        Instantiate(enemies[enemyToSpawn], ship.position + offset, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        StartCoroutine(Spawnar(delay));
    }
    IEnumerator Boss(float delay)
    {

        yield return new WaitForSeconds(delay);
        if(ship == null)
        {
            yield break;
        }
        Vector3 offset = new Vector2(Random.Range(distance.x, distance.y), Random.Range(distance.x, distance.y));
        Instantiate(boss, ship.position + offset, Quaternion.Euler(0,0,Random.Range(0,360)));
    }

}
