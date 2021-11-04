using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodebreakerProceedButton : MonoBehaviour
{
    public delegate void Proceed();
    public delegate bool CheckIfAllowed();

    public Proceed proceed;
    public CheckIfAllowed allowed;
    AudioManager audioManager;
    Animator animator;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        if (allowed())
        {
            animator.SetTrigger("pressed");
            audioManager.PlayButtonPressEvent();
            proceed();
        }
        else
        {
            audioManager.PlayButtonDeniedEvent();
        }
    }
}
