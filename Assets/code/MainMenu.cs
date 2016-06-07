﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button creditsButton;
    public Button helpButton;
    public Button quitButton;
    public Button currentButton;
    public EventSystem eventSystem;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //CheckInput();
    }

    public void LoadLevel(string filename)
    {
        Application.LoadLevel(filename);
    }

    public void Quit()
    {
        Application.Quit();
    }
}