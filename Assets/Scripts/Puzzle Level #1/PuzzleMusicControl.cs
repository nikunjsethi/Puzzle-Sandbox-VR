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
    private bool introStarted;
    public bool introFinished = false;
    private float delayMusic = 6f;
    private float currentTime = 0f;

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

        
        


        laserTR = laser.GetComponent<Transform>();
        laserControl = GetComponent<LaserControl>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        float introLength = intro.clip.length;

        if (currentTime >= delayMusic && !introStarted)
        {
            intro.Play();
            soundtrack.PlayDelayed(introLength);
            introStarted = true;
        }
        

        if (soundtrack.isPlaying)
        {
            introFinished = true;
            laser.Play();
        }

        Vector3 position = laserControl.line.GetPosition(1);

        laser.transform.position = position;


    }
}
