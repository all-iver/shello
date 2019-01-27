using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabWalker : MonoBehaviour
{

    [Range(0, 5)]
    public float speed;
    public float yShift = 1.5f;
    //move left and right from the start point
    public float xShift = 0f;
    private Vector3 startV3Pos;

   

    void Start()
    {
        startV3Pos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 v = startV3Pos;
        v.y += yShift * Mathf.Sin(Time.time * speed);
        v.x += xShift * Mathf.Sin(Time.time * speed);
        transform.position = v;
        
    }


}