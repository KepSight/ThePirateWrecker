using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public int cannonID;
    float rotationZ;
    Vector3 target, difference;
    public Transform fixo;
    public float spd = 100;
    public float fac;
    Quaternion assist;
    Quaternion auxA, auxB;
    Ship ship;
    private void Start()
    {
        ship = GetComponentInParent<Ship>();
        fac = 0.5f;
    }
    private void Update()
    {
        if(ship.actCannon != cannonID)
        {
            return;
        }
        target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        difference = target - transform.position;
        rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        assist = Quaternion.Euler(0, 0, rotationZ);

         if(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, assist.eulerAngles.z) > 2)
        {
            fac -= Time.deltaTime * spd;
            fac = Mathf.Clamp(fac, 0, 1);
            auxA = Quaternion.Euler(0, 0, fixo.rotation.eulerAngles.z + 30);
            auxB = Quaternion.Euler(0, 0, fixo.rotation.eulerAngles.z - 30);
            transform.rotation = Quaternion.Lerp(auxA, auxB, fac);
        }
        else if (Mathf.DeltaAngle(transform.rotation.eulerAngles.z, assist.eulerAngles.z) < -2)
        {
            fac += Time.deltaTime * spd;
            fac = Mathf.Clamp(fac, 0, 1);
            auxA = Quaternion.Euler(0, 0, fixo.rotation.eulerAngles.z+30);
            auxB = Quaternion.Euler(0, 0, fixo.rotation.eulerAngles.z - 30);
            transform.rotation = Quaternion.Lerp(auxA, auxB, fac);
        }
        else
        {
            transform.rotation = assist;
        }

    }

}
