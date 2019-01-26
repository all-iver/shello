using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetSand : MonoBehaviour
{

    [Header("WetSand")]
    public SpriteRenderer wetSandRenderer;
    public float fadeDuration;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Lerp_MeshRenderer_Color(wetSandRenderer, fadeDuration, Color.white, Color.clear));
    }

    private IEnumerator Lerp_MeshRenderer_Color(SpriteRenderer target_SpriteRenderer, float
       lerpDuration, Color startLerp, Color targetLerp)
    {
        Debug.Log("lerping to clear");
        float lerpStart_Time = Time.time;
        float lerpProgress;
        bool lerping = true;
        while (lerping)
        {
            yield return new WaitForEndOfFrame();
            lerpProgress = Time.time - lerpStart_Time;
            if (target_SpriteRenderer != null)
            {
                target_SpriteRenderer.material.color = Color.Lerp(startLerp, targetLerp, lerpProgress / lerpDuration);
            }
            else
            {
                lerping = false;
            }


            if (lerpProgress >= lerpDuration)
            {
                lerping = false;
            }
        }
        Destroy(gameObject);
        yield break;
    }

}
