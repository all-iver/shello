using UnityEngine;
using System.Collections.Generic;

public class GameSounds : MonoBehaviour {

    public static GameSounds instance;

    [System.Serializable]
    public class SoundFiles {
        public AudioClip beep, goBeep, eggCrack;
    }
    public SoundFiles sounds; 
    private List<AudioSource> sources;
    public AudioSource sourcePrefab;
    
    void Awake() {
        sources = new List<AudioSource>();
        instance = this;
    }
    
    public void PlayClip(AudioClip clip) {
        AudioSource source = null;
        for (int i = 0; i < sources.Count; i++) {
            if (!sources[i].isPlaying) {
                source = sources[i];
                break;
            }
        }
        if (!source) {
            source = Instantiate(sourcePrefab);
            source.transform.SetParent(transform);
            source.playOnAwake = false;
            sources.Add(source);
        }
        source.clip = clip;
        source.Play();
    }

    public void PlayBeep()
    {
        PlayClip(sounds.beep);
    }

    public void PlayGoBeep()
    {
        PlayClip(sounds.goBeep);
    }

    public void PlayEggCrack() {
        PlayClip(sounds.eggCrack);
    }

}
