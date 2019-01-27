using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public Sprite[] sprites;
    SpriteRenderer sr;
    float shakeInterval = 0.05f;
    float shakeDistance = 0.03f;
    float shakeTimer;
    float hatchInterval = 0.3f;
    Vector3 homePos;
    public bool shaking;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        homePos = transform.position;
        shakeTimer = 0;
        Reset();
    }

    public void Reset() {
        StopAllCoroutines();
        sr.sprite = sprites[0];
        transform.position = homePos;
        shaking = false;
    }

    void Shake() {
        if (transform.position.x < homePos.x) {
            transform.position = new Vector3(
                    homePos.x + shakeDistance,
                    homePos.y,
                    homePos.z);
        } else {
            transform.position = new Vector3(
                    homePos.x - shakeDistance,
                    homePos.y,
                    homePos.z);
        }
        // transform.position = new Vector3(
        //         homePos.x + Random.Range(-shakeDistance, shakeDistance), 
        //         homePos.y + Random.Range(-shakeDistance, shakeDistance),
        //         homePos.z);
    }

    IEnumerator DoHatch(GameObject spawn) {
        spawn.transform.position = transform.position;
        spawn.SetActive(false);
        shaking = true;
        foreach (Sprite s in sprites) {
            sr.sprite = s;
            yield return new WaitForSeconds(hatchInterval);
        }
        GameSounds.instance.PlayEggCrack();
        shaking = false;
        spawn.SetActive(true);
    }

    public void Hatch(GameObject spawn) {
        StartCoroutine(DoHatch(spawn));
    }

    void Update() {
        if (!shaking)
            return;
        shakeTimer += Time.deltaTime;
        if (shakeTimer >= shakeInterval) {
            shakeTimer = 0;
            Shake();
        }
    }

}
