using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullShadow : MonoBehaviour
{
    //public GameObject seagullShadow;
    public SpriteRenderer spriteRenderer;

    public Transform[] startingPos;
	public Transform[] targetPos;

	public float speed;
    public float waitTime;

	//public bool movingLeft;
	//public Transform currentPos;
	public Transform target;
    public float step;
    public int x = 0;

    void Start()
    {
        StartCoroutine("WaitAndFly");
       //spriteRenderer = seagullShadow.GetComponent<SpriteRenderer>();
    }


    private IEnumerator WaitAndFly()
    {
        while (true)
        {
			UpdateSeagull();
			yield return new WaitForSeconds(waitTime);
            x++;
            //movingLeft = !movingLeft; //toggle
        }
    }

    void UpdateSeagull()
    {
        if (x >= startingPos.Length)
        {
            x = 0;
        }
	
		if (x == 1)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;
		}

		transform.position = startingPos[x].position;
		target = targetPos[x];
	}

    private void Update()
    {
        step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
	}
}





