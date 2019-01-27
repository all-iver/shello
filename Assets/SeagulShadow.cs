using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagulShadow : MonoBehaviour
{
    public GameObject seagulShadow;
    private SpriteRenderer spriteRenderer;

    public Transform pos1;
    public Transform pos2;
    public Transform pos3;
    public Transform pos4;

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
        if (x == 5)
        {
            x = 0;
        }

        if (x == 0)
        {
            target = pos1;
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        if (x == 1)
        {
            target = pos2;
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = false;
        }

        if (x == 3)
        {
            target = pos3;
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }

        if (x == 4)
        {
            target = pos4;
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





