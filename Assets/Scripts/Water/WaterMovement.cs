using System.Collections;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    [Range(0, 5)]
    public float speed;
    public float yShift = 1.5f;  // Amount to move left and right from the start point
    public float xShift = 0f;
    private Vector3 startV3Pos;

    [Header("Wet Sand")]
    public GameObject wetSandPrefab;
    public bool baseWaterLayer;
    public bool isGoingOut;
    private float prevPos;

    void Start()
    {
        startV3Pos = transform.position;
    }

    void Update()
    {
        Vector3 v = startV3Pos;
        v.y += yShift * Mathf.Sin(Time.time * speed);
        v.x += xShift * Mathf.Sin(Time.time * speed);
        transform.position = v;

        if (baseWaterLayer)
        {
            if (!isGoingOut)
            {
                if (prevPos < transform.position.y)
                {
                    isGoingOut = true;
                    //Debug.Log("water is NOW going out");
                    Instantiate(wetSandPrefab, transform.position, transform.rotation);
                }
            }

            if (isGoingOut)
            {
                if (prevPos > transform.position.y)
                {
                    isGoingOut = false;
                    //Debug.Log("water is NOW going in");
                }
            }

            prevPos = v.y;
        }
    }
}
