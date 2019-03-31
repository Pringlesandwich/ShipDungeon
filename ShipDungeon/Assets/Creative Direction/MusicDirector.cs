using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDirector : MonoBehaviour {

    public AudioClip Alarm;
    public AudioClip Bass;
    public AudioClip Drums;
    public AudioClip Plasma;
    public AudioClip Ring;

   // public AudioSource AlarmAudioSource;

    // Use this for initialization
    void Start () {
        var alarm = this.gameObject.AddComponent<AudioSource>();
        alarm.name = "AlarmSource";
        alarm.clip = Alarm;
        alarm.loop = true;
        alarm.Play();

        var bass = this.gameObject.AddComponent<AudioSource>();
        bass.name = "BassSource";
        bass.clip = Bass;
        bass.loop = true;
        bass.Play();

        var drums = this.gameObject.AddComponent<AudioSource>();
        drums.name = "DrumsSource";
        drums.clip = Drums;
        drums.loop = true;
        drums.Play();

        var ring = this.gameObject.AddComponent<AudioSource>();
        ring.name = "RingSource";
        ring.clip = Ring;
        ring.loop = true;
        ring.volume = 0.66f;
        ring.Play();

        var plasma = this.gameObject.AddComponent<AudioSource>();
        plasma.name = "PlasmaSource";
        plasma.clip = Plasma;
        plasma.loop = true;
        plasma.volume = 0.33f;
        plasma.Play();


    }

    // Update is called once per frame
    void Update () {
		
	}
}
