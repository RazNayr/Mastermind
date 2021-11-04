using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegHole : MonoBehaviour
{
    public GameObject selectedOutline;
    private bool selected = false;
    private GameObject placedPeg = null;
    private float verticalOffset = 0.23f;

    public void ToggleSelection()
    {
        if (selected)
            selectedOutline.SetActive(false);
        else
            selectedOutline.SetActive(true);

        selected = !selected;
    }

    public void PlacePeg(GameObject newPeg)
    {
        if (placedPeg)
            Destroy(placedPeg);

        newPeg.transform.position = transform.position;
        newPeg.transform.parent = transform;
        newPeg.transform.localPosition += new Vector3(0, verticalOffset, 0);
        placedPeg = newPeg;
    }

    public void Reset()
    {
        if (placedPeg)
            Destroy(placedPeg);

        if(selected)
            selectedOutline.SetActive(false);

        selected = false;
    }
}
