using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSpriteElements : MonoBehaviour
{
    private SpriteRenderer sp;

    [Header("Elements to Randomize:")]
    public Sprite[] SpriteArray;
    private Sprite currentSprite;
    public bool randomlyFlipX = false;
    public bool randomlyFlipY = false;

    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        int random = Random.Range(0, SpriteArray.Length);
        currentSprite = SpriteArray[random];

        if (SpriteArray != null)
        {
            sp.sprite = currentSprite;
        }
    
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
