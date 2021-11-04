using System.Collections.Generic;
using System.Linq;

public class BullsAndCows
{
    public int bulls = 0;
    public int cows = 0;

    public static char bullCharacter = 'B';
    public static char cowCharacter = 'C';

    public string Descriptor()
    {
        return new string(bullCharacter, bulls) + new string(cowCharacter, cows);
    }

    // Function to calculate the resulting bulls and cows when comparing two codes
    public static BullsAndCows GetBullsAndCows(string code1, string code2)
    {
        BullsAndCows bullsAndCows = new BullsAndCows();

        if (code1 == code2)
        {
            bullsAndCows.bulls = 4;
        }
        else
        {
            List<char> remainingDigits1 = code1.ToCharArray().ToList();
            List<char> remainingDigits2 = code2.ToCharArray().ToList();

            for (int i = 0; i < GameController.lengthOfCode; i++)
            {
                if (code1[i] == code2[i])
                {
                    remainingDigits1.Remove(code1[i]);
                    remainingDigits2.Remove(code1[i]);
                    bullsAndCows.bulls++;
                }
            }

            foreach (char remainingDigit1 in remainingDigits1)
            {
                if (remainingDigits2.Contains(remainingDigit1))
                {
                    remainingDigits2.Remove(remainingDigit1);
                    bullsAndCows.cows++;
                }
            }
        }

        return bullsAndCows;
    }
}