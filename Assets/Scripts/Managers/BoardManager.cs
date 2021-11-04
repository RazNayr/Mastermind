using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Routine to execute when the player codebreaker makes his final guess
    public delegate void EndingRoutine(bool actualCodefound);
    public EndingRoutine endingRoutine;

    // Peg Components
    [SerializeField] GameObject pegPrefab;
    [SerializeField] GameObject bullPegPrefab;
    [SerializeField] GameObject cowPegPrefab;
    List<SelectablePeg> pegsInScene;
    List<PegDetails> pegDetails;

    [Header("Codemaker Objects")]
    CodemakerProceedButton codemakerProceedButton;
    [SerializeField] GameObject actualCodeFlap;
    [SerializeField] CodemakerPegRow codemakerPegRow;

    [Header("Codebreaker Objects")]
    CodebreakerProceedButton codebreakerProceedButton;
    [SerializeField] List<CodebreakerPegRow> codebreakerPegRows;

    // Board Parameters
    SortedDictionary<int, char> positionedDigits;
    PegRow selectableRow;
    PegHole selectedHole;
    bool selectionAllowed = false;

    // Managers
    AudioManager audioManager;

    private void Awake()
    {
        InitialisePegComponents();
        InitialiseProceedButtons();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void InitialisePegComponents()
    {
        pegDetails = FileHandler.RetrieveAllPegDetails();
        pegsInScene = FindObjectsOfType<SelectablePeg>().ToList();
    }

    private void InitialiseProceedButtons()
    {
        // Initialise proceed buttons with required delegates
        codemakerProceedButton = FindObjectOfType<CodemakerProceedButton>();
        codemakerProceedButton.proceed = GenerateCodeFromPegs;
        codemakerProceedButton.allowed = CodemakerCanProceed;

        // Initialise proceed buttons with required delegates
        codebreakerProceedButton = FindObjectOfType<CodebreakerProceedButton>();
        codebreakerProceedButton.proceed = GenerateCodeFromPegs;
        codebreakerProceedButton.allowed = CodebreakerCanProceed;
    }

    public void InitialiseBoard(GameType gametype)
    {
        if (gametype == GameType.CODEMAKER)
            selectableRow = codemakerPegRow;

        if (gametype == GameType.CODEBREAKER)
            selectableRow = codebreakerPegRows[0];

        ResetBoard();
        selectedHole = null;
        positionedDigits = new SortedDictionary<int, char>();
        selectionAllowed = true;
    }

    private void ResetBoard()
    {
        // Reset Button Positions
        codemakerProceedButton.Reset();

        // Reset Codebreaker Peg Holes
        foreach (CodebreakerPegRow pegRow in codebreakerPegRows)
        {
            // Reset guess peg holes
            pegRow.pegHoles.Select(h => { h.Reset(); return h; }).ToList();

            // Reset results peg holes
            pegRow.resultPegHoles.Select(h => { h.Reset(); return h; }).ToList();
        }

        // Reset Codemaker Peg Holes
        codemakerPegRow.pegHoles.Select(h => { h.Reset(); return h; }).ToList();

        // Reset Codemaker Flap
        actualCodeFlap.SetActive(true);
    }

    public void DisablePlayerInput()
    {
        selectionAllowed = false;
    }

    public void EnablePlayerInput()
    {
        selectionAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // If player input is allowed
        if (selectionAllowed)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (selectableRow.pegHoles.Any(h => hit.transform == h.transform))
                {
                    PegHole selectedHole = selectableRow.pegHoles.FirstOrDefault(h => hit.transform == h.transform);
                    HandlePegHoleSelection(selectedHole);
                }

                if(pegsInScene.Any(p => hit.transform == p.transform) && selectedHole != null)
                {
                    SelectablePeg selectedPeg = pegsInScene.FirstOrDefault(p => hit.transform.position == p.transform.position);
                    PlacePeg(selectedPeg);
                }
            }
        }
    }

    private void HandlePegHoleSelection(PegHole newHole)
    {
        // Toggle previously selected hole
        if (selectedHole != null)
            selectedHole.ToggleSelection();

        // Turn on outline for new selected hole
        newHole.ToggleSelection();

        // Keep track of current hole in selection
        selectedHole = newHole;

        // Play audio event
        audioManager.PlayPegHoleSelectionEvent();
    }

    private void PlacePeg(SelectablePeg selectedPeg)
    {
        Material selectedPegMaterial = selectedPeg.GetPegMaterial();
        print(selectedPegMaterial);
        print(selectedPegMaterial.color);

        // Retrieve equivalent peg details scriptable object for the selected peg
        PegDetails selectedPegDetails = pegDetails.FirstOrDefault(pd => pd.pegMaterial.color == selectedPegMaterial.color);

        // Instantiate new peg and apply the same material as that of selected peg
        GameObject newPeg = Instantiate(pegPrefab);
        newPeg.GetComponent<MeshRenderer>().material = selectedPegDetails.pegMaterial;

        // Get position of selected hole
        int holePosition = selectableRow.pegHoles.IndexOf(selectedHole);

        // Assign selected hole position with chosen digit 
        positionedDigits[holePosition] = selectedPegDetails.digit;

        // Place new peg in selected hole
        selectedHole.PlacePeg(newPeg);

        // Play audio Event
        audioManager.PlayPegPlacementEvent();
    }

    // Filter selectable pegs to show in scene based on the number of inputs
    public void ShowOnlyRequiredPegs()
    {
        List<PegDetails> pegTypesToRemove = pegDetails.Where(pd => int.Parse(pd.digit.ToString()) >= GameController.numInputs).ToList();
        
        foreach (SelectablePeg peg in pegsInScene)
        {
            Material pegMaterial = peg.GetComponent<MeshRenderer>().material;

            if (pegTypesToRemove.Any(p => pegMaterial.name.Contains(p.pegMaterial.name) )){
                peg.gameObject.SetActive(false);
            }
            else
            {
                peg.gameObject.SetActive(true);
            }
        }
    }

    public bool CodemakerCanProceed()
    {
        if (GameController.gametype == GameType.CODEMAKER && positionedDigits.Count == GameController.lengthOfCode)
        {
            return true;
        }
        return false;
    }

    public bool CodebreakerCanProceed()
    {
        if (GameController.gametype == GameType.CODEBREAKER && positionedDigits.Count == GameController.lengthOfCode)
        {
            return true;
        }
        return false;
    }

    // Function called once the user presses the proceed/play button
    // This function generates a code based on the code built from peg selection
    public void GenerateCodeFromPegs()
    {
        string code = "";

        // Build ordered code
        foreach (char digit in positionedDigits.Values)
            code += digit;

        // If hole is currently selected, toggle it off
        if (selectedHole)
        {
            selectedHole.ToggleSelection();
            selectedHole = null;
        }

        // If game type is codemaker, assign constructed code to game controller
        if(GameController.gametype == GameType.CODEMAKER)
        {
            GameController.actualCode = code;
        }
        // If game type is codebreaker, handle the guessing process of the player
        else if(GameController.gametype == GameType.CODEBREAKER)
        {
            // Increment number of guesses made
            GameController.currentGuess++;

            // Retrive the number of bulls and cows and populate codebreaker row with result pegs
            BullsAndCows guessResult = BullsAndCows.GetBullsAndCows(code, GameController.actualCode);
            FillCodebreakerRow(guessResult, code);

            // If no guesses are left or if the user guessed correctly
            if (GameController.currentGuess >= GameController.maxGuesses || guessResult.bulls == GameController.lengthOfCode)
            {
                // Show actual code
                actualCodeFlap.SetActive(false);

                // Disable player input
                DisablePlayerInput();

                // Execute ending routine delegated by the game controller
                endingRoutine(guessResult.bulls == GameController.lengthOfCode);
            }
            // If guesses are left and the user didn't manage to guess the actual code
            else
            {
                // Clear digits for the next row
                positionedDigits.Clear();

                // Assign next row to be selected by the player
                selectableRow = codebreakerPegRows[GameController.currentGuess];
            }
            
        }
        
    }

    
    // Function used to fill the codemaker row with 3D pegs based on the generated code
    public void FillCodemakerRow(string actualCode)
    {
        CodemakerPegRow rowToFill = codemakerPegRow;

        // Place Pegs
        for (int i = 0; i < GameController.lengthOfCode; i++)
        {
            PegDetails guessPegDetails = pegDetails.FirstOrDefault(pd => pd.digit == actualCode[i]);

            GameObject guessPeg = Instantiate(pegPrefab);
            guessPeg.GetComponent<MeshRenderer>().material = guessPegDetails.pegMaterial;
            rowToFill.pegHoles[i].PlacePeg(guessPeg);
        }
    }

    // Function to fill codebreaker row with guess pegs and bulls and cows score pegs
    public void FillCodebreakerRow(BullsAndCows guessResult, string guess)
    {
        List<char> positionedGuessDigits = guess.ToCharArray().ToList();
        List<char> bullsAndCowsDescriptor = guessResult.Descriptor().ToCharArray().ToList();
        CodebreakerPegRow rowToFill = codebreakerPegRows[GameController.currentGuess - 1];

        // Place Guess Pegs
        for (int i = 0; i < GameController.lengthOfCode; i++)
        {
            PegDetails guessPegDetails = pegDetails.FirstOrDefault(pd => pd.digit == positionedGuessDigits[i]);

            GameObject guessPeg = Instantiate(pegPrefab);
            guessPeg.GetComponent<MeshRenderer>().material = guessPegDetails.pegMaterial;
            rowToFill.pegHoles[i].PlacePeg(guessPeg);
        }

        // Place Bulls And Cows
        for (int i = 0; i < bullsAndCowsDescriptor.Count; i++)
        {
            char resultType = bullsAndCowsDescriptor[i];
            PegHole resultPegHole = rowToFill.resultPegHoles[i];

            if (resultType == BullsAndCows.bullCharacter)
                resultPegHole.PlacePeg(Instantiate(bullPegPrefab));
            else
                resultPegHole.PlacePeg(Instantiate(cowPegPrefab));
        }
    }
}
