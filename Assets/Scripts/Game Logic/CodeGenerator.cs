using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CodeGenerator
{
    static List<string> allPossibleCodes;

    // Function to generate all codes using digits 0 to 9
    public static void GenerateAllPossibleCodes()
    {
        // Total number of different pegs
        int maxInput = 10;

        // Total number of digits within a code
        int lengthOfCode = GameController.lengthOfCode;

        // Maximum number of codes that can be guessed
        int totalPermutations = (int)Mathf.Pow(maxInput, lengthOfCode);

        // String template used to fill in leading spaces with zero characters for integers < 1000
        string codeTemplate = new string('0', lengthOfCode);

        // Create all possible codes in string format
        allPossibleCodes = Enumerable.Range(0, totalPermutations).Select(v => v.ToString(codeTemplate)).ToList();
    }

    // Function to retrieve a subset of codes from the set of all generated codes
    // Allows mastermind to have any number of pegs as inputs within the range of 1 to 10
    public static List<string> GetSetOfPossibleCodes()
    {
        // Number of different pegs
        int numInputs = GameController.numInputs;

        // Total number of digits within a code
        int lengthOfCode = GameController.lengthOfCode;

        // Return complete set if number of inputs is maxed
        if (numInputs == 10)
        {
            return allPossibleCodes.ToList();
        }
        else
        {
            // If num input is 6, max digit code would be 5555 since 0 is inclusive
            char largestDigit = (numInputs - 1).ToString()[0];
            int maxDigitCode = int.Parse(new string(largestDigit, lengthOfCode));

            // List of digits that cannot be used
            List<string> unusedDigits = Enumerable.Range(numInputs, 10 - numInputs).Select(d => d.ToString()).ToList();

            // Return subset of codes that do not include unused digits
            return allPossibleCodes.Where(c => int.Parse(c) <= maxDigitCode && !unusedDigits.Any(d => c.Contains(d))).ToList();
        }
    }

    // Function used to generate a random code from the set of allowed codes
    public static string GenerateRandomCode()
    {
        List<string> possibleCodes = GetSetOfPossibleCodes();
        int randomCodePos = Random.Range(0, possibleCodes.Count);
        return possibleCodes[randomCodePos];
    }
}
