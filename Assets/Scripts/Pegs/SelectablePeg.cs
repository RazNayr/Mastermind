using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script attached to every peg in scene to retrieve the material of the peg
public class SelectablePeg : MonoBehaviour
{
    Material pegMaterial;

    // Start is called before the first frame update
    void Start()
    {
        pegMaterial = GetComponent<MeshRenderer>().material;
    }

    public Material GetPegMaterial()
    {
        return pegMaterial;
    }
}
