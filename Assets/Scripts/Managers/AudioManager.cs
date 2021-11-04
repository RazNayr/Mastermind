using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource uiAudioSource;
    [SerializeField] AudioSource boardAudioSource;
    [SerializeField] AudioSource miscAudioSource;

    [Header("Board Clips")]
    [SerializeField] AudioEvent buttonPressEvent;
    [SerializeField] AudioEvent buttonDeniedEvent;
    [SerializeField] AudioEvent pegHoleSelectionEvent;
    [SerializeField] AudioEvent pegPlacementEvent;

    [Header("UI Clips")]
    [SerializeField] AudioEvent uiButtonPressEvent;

    [Header("Other Clips")]
    [SerializeField] AudioEvent cameraSwooshEvent;
    [SerializeField] AudioEvent cameraSwooshShortEvent;

    private void Start()
    {
        InitialiseButtonSounds();
    }

    private void InitialiseButtonSounds()
    {
        List<Button> allUIButtons = FindObjectsOfType<Button>().ToList();
        foreach (Button uiButton in allUIButtons)
        {
            uiButton.onClick.AddListener(PlayUIButtonPressEvent);
        }
    }

    public void PlayButtonPressEvent()
    {
        buttonPressEvent.Play(boardAudioSource);
    }

    public void PlayButtonDeniedEvent()
    {
        buttonDeniedEvent.Play(boardAudioSource);
    }

    public void PlayPegHoleSelectionEvent()
    {
        pegHoleSelectionEvent.Play(boardAudioSource);
    }
    public void PlayPegPlacementEvent()
    {
        pegPlacementEvent.Play(boardAudioSource);
    }
        
    public void PlayUIButtonPressEvent()
    {
        uiButtonPressEvent.Play(uiAudioSource);
    }

    public void PlayCameraSwooshEvent()
    {
        cameraSwooshEvent.Play(miscAudioSource);
    }

    public void PlayCameraSwooshShortEvent()
    {
        cameraSwooshShortEvent.Play(miscAudioSource);
    }
}
