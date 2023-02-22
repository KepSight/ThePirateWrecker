using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    Transform ship;
    Vector3 mousePos, targetPos;
    Camera cam;
    public float limit;
    float limitBase;
    bool trava;
    public float speed;
    private void Start()
    {
        ship = FindObjectOfType<Ship>().transform;
        limitBase = limit;
        cam = Camera.main;
    }
    private void Update()
    {
        if(ship == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(2))
        {
            if (trava)
            {
                limit = limitBase;
                trava = false;
            }
            else
            {
                limit = 0;
                trava = true;
            }
        }
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        
    }
    private void FixedUpdate()
    {
        if (ship == null)
        {
            return;
        }
        targetPos = (ship.position + mousePos) / 2f;
        targetPos.x = Mathf.Clamp(targetPos.x, -limit + ship.position.x, limit + ship.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -limit + ship.position.y, limit + ship.position.y);
        targetPos.z = -10;
        transform.position = Vector3.Lerp(transform.position,targetPos, Time.deltaTime * speed);
    }
}
