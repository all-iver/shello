using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagulShadow : MonoBehaviour
{
    public GameObject seagulShadow;
    private SpriteRenderer spriteRenderer;

    public Transform[] positions;

    public float speed;
    public float waitTime;

    //public bool movingLeft;

    public Transform target;
    public float step;
    public int x = 0;

    void Start()
    {
        StartCoroutine("WaitAndFly");
        spriteRenderer = seagulShadow.GetComponent<SpriteRenderer>();
    }


    private IEnumerator WaitAndFly()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            x++;
            UpdateSeagull();
            //movingLeft = !movingLeft; //toggle
        }
    }

    void UpdateSeagull()
    {
        if (x >= positions.Length)
        {
            x = 0;
        }

        if (x == 0)
        {
            target = positions[x];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        if (x == 1)
        {
            target = positions[x];
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
        }

        if (x == 2)
        {
            target = positions[x];
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        if (x == 3)
        {
            target = positions[x];
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
        }

        if (x == 4)
        {
            target = positions[x];
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = false;
        }
    }

    private void Update()
    {
        step = speed * Time.deltaTime; // calculate distance to move
        seagulShadow.transform.position = Vector3.MoveTowards(seagulShadow.transform.position, target.position, step);
    }
}





