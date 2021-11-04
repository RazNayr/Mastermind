using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidController : MonoBehaviour
{
    [SerializeField] Animator codemakerAnimator;
    [SerializeField] Animator codebreakerAnimator;
    
    public void MakeCodebreakerClap()
    {
        codebreakerAnimator.SetTrigger("Clap");
    }

    public void MakeCodebreakerCelebrate()
    {
        codebreakerAnimator.SetTrigger("Celebrate");
    }

    public void MakeCodemakerClap()
    {
        codemakerAnimator.SetTrigger("Clap");
    }

    public void MakeCodemakerCelebrate()
    {
        codemakerAnimator.SetTrigger("Celebrate");
    }
}
