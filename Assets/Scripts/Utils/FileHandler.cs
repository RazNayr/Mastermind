using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FileHandler
{
    // Retrieve all peg details scriptable objects within the resources folder
    public static List<PegDetails> RetrieveAllPegDetails()
    {
        List<PegDetails> allPegDetails = Resources.LoadAll<PegDetails>("Pegs").ToList();
        return allPegDetails;
    }
}