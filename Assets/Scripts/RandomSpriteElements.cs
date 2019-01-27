using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSpriteElements : MonoBehaviour
{
    private SpriteRenderer sp;

    [Header("Elements to Randomize:")]
    public bool randomlyFlipX = false;
    public bool randomlyFlipY = false;


    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();

        if (randomlyFlipX == true)
        {
            var myBool = (Random.value < 0.5);
            sp.flipX = myBool;
            }

        if (randomlyFlipY == true)
        {
            var myBool = (Random.value < 0.5);
            sp.flipY = myBool;
        }
    }
}
