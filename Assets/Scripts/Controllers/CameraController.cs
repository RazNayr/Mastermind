using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Animator cameraAnimator;
    [SerializeField] AnimationClip codemakerLookUp;

    private void Start()
    {
        cameraAnimator = GetComponent<Animator>();
    }

    public void GoToCodemaker()
    {
        cameraAnimator.SetTrigger("GoToCodemaker");
    }

    public void GoToCodebreaker()
    {
        cameraAnimator.SetTrigger("GoToCodebreaker");
    }

    public float CodemakerLookUp()
    {
        cameraAnimator.SetTrigger("Lookup");
        return codemakerLookUp.length;
    }

    public float CodebreakerLookUp()
    {
        cameraAnimator.SetTrigger("Lookup");
        return codemakerLookUp.length;
    }

    public void GoToStartingPosition()
    {
        cameraAnimator.SetTrigger("GoBack");
    }
}
