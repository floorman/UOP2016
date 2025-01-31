﻿using UnityEngine;
using System.Collections.Generic;

#region Requirements
[RequireComponent(typeof(AudioSource))]
#endregion

public class TannoySystem : MonoBehaviour
{
    #region Fields
    public List<AudioClip> announcements;

    private AudioSource mAudioSource;
    public AudioClip cleanup2;
    public AudioClip cleanup6;
    public AudioClip horseMeat;
    public AudioClip danDruff;
    public AudioClip emmaRoyds;
    public AudioClip coreyAnder;
    public AudioClip notPaidEnough;
    public AudioClip videoGames;
    public AudioClip foodGo;
    public AudioClip oldLady;
    public AudioClip enoughFood;

    private float mMinWaitTime = 15.0f;
    private float mMaxWaitTime = 30.0f;
    private float mTimer;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        mAudioSource = this.GetComponent<AudioSource>();
    }

    private void Start()
    {
        mTimer = Random.Range(mMinWaitTime, mMaxWaitTime);

        announcements.Add(cleanup2);
        announcements.Add(cleanup6);
        announcements.Add(horseMeat);
        announcements.Add(danDruff);
        announcements.Add(emmaRoyds);
        announcements.Add(coreyAnder);
        announcements.Add(notPaidEnough);
        announcements.Add(videoGames);
        announcements.Add(foodGo);
        announcements.Add(oldLady);
        announcements.Add(enoughFood);
    }

    private void Update()
    {
        if (mTimer >= 0.0f)
        {
            mTimer -= Time.deltaTime;
        }
        else
        {
            if (!mAudioSource.isPlaying)
            {
                mAudioSource.clip = announcements[Random.Range(0, announcements.Count)];
                mAudioSource.Play();
            }
        }
    }
    #endregion
}
