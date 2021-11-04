using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodemakerProceedButton : MonoBehaviour
{
    public delegate void Proceed();
    public delegate bool CheckIfAllowed();

    public Proceed proceed;
    public CheckIfAllowed allowed;
    Animator animator;
    AudioManager audioManager;
    bool pressed = false;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        if (!pressed && allowed())
        {
            pressed = true;
            animator.SetBool("pressed", true);
            audioManager.PlayButtonPressEvent();
            proceed();
        }
        else
        {
            audioManager.PlayButtonDeniedEvent();
        }
    }
    public void Reset()
    {
        pressed = false;
        animator.SetBool("pressed", false);
    }


}
