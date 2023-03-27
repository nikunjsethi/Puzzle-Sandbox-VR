using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PuzzleMusicControl : MonoBehaviour
{
    private LaserControl laserControl;

    //Audio Sources for Music
    public AudioSource intro;
    public AudioSource soundtrack;
    public AudioSource laser;
    private AudioMixer mixer;
    private AudioMixerGroup gameSounds;
    //Bool to check if into finished playing
    public bool introFinished = false;

    //Laser sound position
    private Transform laserTR;

    // Start is called before the first frame update
    void Start()
    {
        mixer = Resources.Load("AudioMixer") as AudioMixer;
        gameSounds = mixer.FindMatchingGroups("GameSounds")[0];

        AudioSource[] sources = { intro, soundtrack, laser };

        foreach (AudioSource source in sources)
        {
            source.outputAudioMixerGroup = gameSounds; 
        }

        float introLength = intro.clip.length;
        intro.Play();
        soundtrack.PlayDelayed(introLength);


        laserTR = laser.GetComponent<Transform>();
        laserControl = GetComponent<LaserControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (soundtrack.isPlaying)
        {
            introFinished = true;
            laser.Play();
        }

        Vector3 position = laserControl.line.GetPosition(1);

        laser.transform.position = position;


    }
}
