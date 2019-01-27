using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour
{
    Dictionary<int, int> ownership = new Dictionary<int, int>();
    public Transform eggs, ai;
    public float aiSpawnRate = 0.5f;
    public bool spawnAI;
    public GameObject aiPrefab;
    float spawnTimer;

    public int GetNumEggs() {
        return eggs.childCount;
    }

    public void ResetAI() {
        Debug.Log("Reset nest");
        spawnAI = false;
        spawnTimer = 0;
        for (int i = 0; i < eggs.childCount; i++) {
            eggs.GetChild(i).GetComponent<Egg>().Reset();
            // eggs.GetChild(i).gameObject.SetActive(false);
            if (!ownership.ContainsKey(i))
                ownership[i] = 0;
            if (ownership[i] == 2)
                ownership[i] = 0;
        }
        Debug.Log("Removing AI turtles");
        while (ai.childCount > 0) {
            GameObject go = ai.GetChild(0).gameObject;
            go.transform.SetParent(null);
            Destroy(go);
        }
    }

    public int ClaimRandomEgg(bool isAI = false) {
        int start = Random.Range(0, eggs.childCount);
        for (int i = 0; i < eggs.childCount; i++) {
            int c = start + i;
            if (c >= eggs.childCount)
                c -= eggs.childCount;
            if (ownership[c] == 0) {
                ownership[c] = isAI ? 2 : 1;
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
        if (ownership[egg] == 2)
            throw new System.Exception("Trying to release an AI egg");
        Debug.Log("Released egg at " + egg);
        ownership[egg] = 0;
        eggs.GetChild(egg).GetComponent<Egg>().Reset();
        // eggs.GetChild(egg).gameObject.SetActive(false);
    }

    void Update() {
        if (!spawnAI)
            return;
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= aiSpawnRate) {
            int e;
            try {
                e = ClaimRandomEgg(true);
            } catch {
                return; // no eggs left
            }
            GameObject go = Instantiate(aiPrefab);
            go.transform.SetParent(ai, true);
            GameObject egg = GetEggAtIndex(e);
            egg.GetComponent<Egg>().Hatch(go);
            spawnTimer = 0;
        }
    }

}
