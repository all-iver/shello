using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    Dictionary<int, bool> ownership = new Dictionary<int, bool>();
    public Transform eggs;

    void Start() {
        Reset();
    }

    public void Reset() {
        Debug.Log("Reset nest");
        for (int i = 0; i < eggs.childCount; i++) {
            eggs.GetChild(i).gameObject.SetActive(false);
            ownership[i] = false;
        }
    }

    public int GetRandomEggIndex() {
        int start = Random.Range(0, eggs.childCount);
        for (int i = 0; i < eggs.childCount; i++) {
            int c = start + i;
            if (c >= eggs.childCount)
                c -= eggs.childCount;
            if (!ownership[c]) {
                ownership[c] = true;
                Debug.Log("Got egg at " + c);
                return c;
            }
        }
        throw new System.Exception("No eggs left");
    }

    public GameObject GetEggAtIndex(int eggIndex) {
        return eggs.GetChild(eggIndex).gameObject;
    }

    public void ReleaseEgg(int egg) {
        Debug.Log("Released egg at " + egg);
        ownership[egg] = false;
        eggs.GetChild(egg).gameObject.SetActive(false);
    }

}
