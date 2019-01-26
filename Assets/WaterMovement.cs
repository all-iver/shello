using System.Collections;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public float speed;
    bool comingIn;

    public GameObject wetSandPrefab;

    void Update()
    {
        float step = speed * Time.deltaTime;

        if (comingIn)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos.position, step);
        }
        if (transform.position == endPos.position)
        {
            comingIn = false;
            wetSandPrefab.SetActive(true);
        }
        if (!comingIn)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos.position, step);
        }
        if (transform.position == startPos.position)
        {
            comingIn = true;
        }
    }

   

}
