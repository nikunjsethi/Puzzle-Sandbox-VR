using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PuzzleMusicControl : MonoBehaviour
{
    //private LaserControl laserControl;

    //Audio Sources for Music
    public AudioSource intro;
    public AudioSource soundtrack;
    
    private AudioMixer mixer;
    private AudioMixerGroup gameSounds;
    //Bool to check if into finished playing
    public bool introFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        mixer = Resources.Load("AudioMixer") as AudioMixer;
        gameSounds = mixer.FindMatchingGroups("GameSounds")[0];

        AudioSource[] sources = { intro, soundtrack };

        foreach (AudioSource source in sources)
        {
            source.outputAudioMixerGroup = gameSounds; 
        }

        //laserTR = laser.GetComponent<Transform>();
        //laserControl = GetComponent<LaserControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!intro.isPlaying && !introFinished)
        {
            soundtrack.Play();
            introFinished = true;
        }

        //Vector3 position = laserControl.line.GetPosition(1);

        //laser.transform.position = position;


    }
}
