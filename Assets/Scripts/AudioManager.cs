using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager> {

    AudioSource aS;
    AudioClip tone1;
    AudioClip tone2;
    AudioClip explode1;
    AudioClip alert1;
    AudioClip obstB;

    int d = 1;

    int channelsToSpawn = 6;
    //channelFab
    public GameObject channelFab;
    //channelCount List
    List<GameObject> channelList = new List<GameObject>();
    //parent = this

    private void Awake()
    {
        //aS = GetComponent<AudioSource>();
        tone1 = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Tone1.wav");
        tone2 = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Tone2.wav");
        explode1 = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Explosion1.wav");
        alert1 = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/Alert1.wav");
        obstB = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/ObstCD1.wav");
    }

    private void OnEnable()
    {
        EventManager.ChangeDirection += ChangeDirSound;
        EventManager.Restart += Restart;
    }

    private void OnDisable()
    {
        EventManager.ChangeDirection -= ChangeDirSound;
    }

    private void Start()
    {
        aS = GetComponent<AudioSource>();
        

        for (int i = 0; i < channelsToSpawn; i++)    //Populate channel list
        {
            GameObject obj = Instantiate(channelFab, Vector3.zero, Quaternion.identity) as GameObject;
            obj.transform.parent = transform;
            //obj.SetActive(false);
            channelList.Add(obj);
        }
    }

    private void Update()
    {
        //if(GameManager.Instance.Modes == GameManager.Mode.Game && GameManager.Instance.GameMode != GameManager.GameState.GameOver)
        //{
        //    if (aS.isPlaying == false)
        //    {
        //        aS.Play();
        //    }
        //}
        //else
        //{
        //    if (aS.isPlaying == true)
        //    {
        //        aS.Stop();
        //    }
        //}
    }

    void ChangeDirSound()
    {
        AudioSource a = GetChannel();

        if (GameManager.Instance.Modes == GameManager.Mode.Game )
        { //for alternating tone when in game
            d = -d;
            a.clip = d == 1 ? tone1 : tone2;
            //if (!aS.isPlaying) {
            //    aS.Play();
            //}
        }
        else
        { //for consistent tone when NOT in game
            a.clip = tone1;
        }
        a.Play();
    }

    public void PlayObsBounce()
    {
        AudioSource a = GetChannel();
        a.clip = obstB;
        a.Play();
    }

    public void PlayExplode()
    {
        AudioSource a = GetChannel();
        a.clip = explode1;
        a.Play();
    } 

    public void PlayAlert()
    {
        AudioSource a = GetChannel();
        a.clip = alert1;
        a.Play();
    }

    AudioSource GetChannel()
    {
        for (int i = 0; i < channelList.Count; i++) //search the channel object list
        {
            AudioSource channel = channelList[i].GetComponent<AudioSource>();   //get the audiosource component

            if (channel.isPlaying == false)         //check if it's being used
            {
                return channel;                     //if NOT, return that channel
            }
            else
            {
                continue;                           //if it IS, keep looking
            }
        }

        Debug.Log("Error: Not Enough Channels");    //if no available channel is found, more channel objects need spawning
        return null;                                //return null for calling methods
    }

    void Restart()
    {
        StopMain();
    }

    public void PlayMain()
    {
        if (aS.isPlaying == false)
        {
            aS.Play();
        }
    }

    public void StopMain()
    {
        if (aS.isPlaying == true)
        {
            aS.Stop();
        }
    }
}
