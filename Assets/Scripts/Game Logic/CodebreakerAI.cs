using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodebreakerAI
{
    public delegate void EndingRoutine(bool actualCodeFound);
    public EndingRoutine endingRoutine;
    List<string> possibleCodes;

    // Function to remove all codes that do not give the same guess result when compared to the guess made
    private void FilterPossibleAnswers(string guessCode, BullsAndCows guessResult)
    {
        foreach (string possibleAnswer in possibleCodes.ToList())
        {
            BullsAndCows possibleBullsAndCows = BullsAndCows.GetBullsAndCows(guessCode, possibleAnswer);
            if (guessResult.bulls != possibleBullsAndCows.bulls || guessResult.cows != possibleBullsAndCows.cows)
                possibleCodes.Remove(possibleAnswer);
        }
    }

    // Function to apply minimax techniques to find the next guess
    private string MakeGuess()
    {
        string guess;

        // If AI is making the first guess, start with the initial guess of 1122 for a more informative result
        if (GameController.currentGuess == 0)
            guess = "1122";

        // Apply minimax technique
        else if (GameController.guessWithMinimax)
        {
            // Guess with the lowest result frequency
            (string, int)? minGuess = null;

            foreach (string code1 in possibleCodes)
            {
                // Store the total number of codes which produce each bulls and cows result
                Dictionary<string, int> resultFrequency = new Dictionary<string, int>();

                // Iterate through all possible codes to build the resultFrequency table
                foreach (string code2 in possibleCodes)
                {
                    BullsAndCows result = BullsAndCows.GetBullsAndCows(code1, code2);
                    string resultDescriptor = result.Descriptor();

                    if (resultFrequency.ContainsKey(resultDescriptor))
                        resultFrequency[resultDescriptor]++;
                    else
                        resultFrequency[resultDescriptor] = 1;
                }

                // Score of current possible code is the max frequency of scores. 
                // The lower it is, the better. (More informative guess if score is less)
                int score = resultFrequency.Values.Max();

                // Update min guess
                if (minGuess == null)
                    minGuess = (code1, score);
                else if (minGuess.Value.Item2 > score)
                    minGuess = (code1, score);
            }
            guess = minGuess.Value.Item1;
        }

        // Otherwise return first possible code in set
        else
        {
            guess = possibleCodes[0];
        }

        GameController.currentGuess++;
        return guess;
    }

    public IEnumerator StartGuessing(string actualCode, BoardManager boardManager)
    {
        // Initialise set of possible codes
        possibleCodes = CodeGenerator.GetSetOfPossibleCodes();
        bool actualCodeFound = false;

        // Loop until AI makes correct guess or until limit is reached
        while (GameController.currentGuess < GameController.maxGuesses)
        {
            // Make guess and get resulting bulls and cows
            string guessCode = MakeGuess();
            BullsAndCows guessResult = BullsAndCows.GetBullsAndCows(guessCode, actualCode);

            // Place Pegs on board based on AI guess
            yield return new WaitForSeconds(1);
            boardManager.FillCodebreakerRow(guessResult, guessCode);

            if (guessResult.bulls == GameController.lengthOfCode)
            {
                actualCodeFound = true;
                break;
            }

            FilterPossibleAnswers(guessCode, guessResult);
        }

        endingRoutine(actualCodeFound);
    }
}
