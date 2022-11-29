using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Camera[] cameras;
    [SerializeField] Animator deathCameraAnimator;

    [SerializeField] float gameOverDelayTime = 2.0f;

    public List<UIScreen> screens;

    WaitForSeconds gameOverDelay;

    public bool IsInitialized { get; private set; }

    public void Initialize()
    {
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent(out UIScreen screen))
                screens.Add(screen);
        }

        foreach (var s in screens)
            s?.Close();

        cameras[0].enabled = true;

        gameOverDelay = new WaitForSeconds(gameOverDelayTime);

        //GameManager.OnChangeGameState += OnChangeGameState;

        IsInitialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Escape();
        }
    }

    public void Escape() 
    {
        //if (GameManager.Instance.State == GameState.Start)
        //{
        //    ShowScreen<StartScreen>();
        //}
    }

    public void EnableCamera( int cameraIndex )
    {
		for(int i = 0; i < cameras.Length; i++)
            cameras[i].enabled = (i == cameraIndex);
    }

    public T GetScreen<T>()
    {
        UIScreen value = null;

        foreach (var screen in screens)
        {
            if (screen.GetType() == typeof(T))
            {
                value = screen;
            }
        }

        return (T)(object)value;
    }

    public void Show(UIScreen screen)
    {
        ShowScreen(screen);
    }

    public void ShowScreen<T>()
    {
        foreach (var s in screens)
        {
            if (s.GetType() == typeof(T))
                s.Open();
            else 
            {
                if(s.IsOpen)
                    s.Close();
            }
        }
    }

    public void ShowScreen(UIScreen screen)
    {
        foreach (var s in screens)
        {
            if (s.Equals(screen))
                s.Open();
            else
                s.Close();
        }
    }

    public void ShowScreen(Screen[] screens)
    {
        foreach (var s in this.screens)
        {
            if (s.Equals(screens))
                s.Open();
            else
                s.Close();
        }
    }

    public void PrepareTheButtons(string _clothesForTheLevel)
    {
        GameScreen gs = GetScreen<GameScreen>();

        gs.LoadAppearanceButtons(_clothesForTheLevel);
    }

    public void PlayerReachedTheLastLevel()
    {
        foreach(var s in screens) 
            s.Close();
    }
}