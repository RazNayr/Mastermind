using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Game Parameters
    public static GameType gametype;
    public static string actualCode;
    public static int numInputs = 7;
    public static int lengthOfCode = 4;
    public static int maxGuesses = 10;
    public static int currentGuess = 0;
    public static bool guessWithMinimax = true;
    bool inRoutine = false;

    // Managers
    UIManager uiManager;
    BoardManager boardManager;
    AudioManager audioManager;

    // Controllers
    CameraController cameraController;
    HumanoidController humanoidController;
    
    // AI
    CodebreakerAI codebreakerAI;

    private void Awake()
    {
        InitialiseDefaultGameParameters();
        InitialiseObjectReferences();
        DelegateRoutines();
    }

    private void Start()
    {
        boardManager.ShowOnlyRequiredPegs();
    }

    private void InitialiseDefaultGameParameters()
    {
        numInputs = 7;
        lengthOfCode = 4;
        maxGuesses = 10;
        currentGuess = 0;
        guessWithMinimax = true;
        inRoutine = false;
    }

    private void InitialiseObjectReferences()
    {
        // Find Managers
        uiManager = FindObjectOfType<UIManager>();
        boardManager = FindObjectOfType<BoardManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Find Controllers
        cameraController = FindObjectOfType<CameraController>();
        humanoidController = FindObjectOfType<HumanoidController>();
    }

    private void DelegateRoutines()
    {
        // Generate all possible codes at the start of the game
        CodeGenerator.GenerateAllPossibleCodes();

        // Create an AI object
        codebreakerAI = new CodebreakerAI();

        // Delegate the ending routine to both the ai and the board manager
        codebreakerAI.endingRoutine = StartEndingRoutine;
        boardManager.endingRoutine = StartEndingRoutine;
    }

    // Function called by the options slider to update the number of inputs
    public void UpdateNumInputs(System.Single inputs)
    {
        numInputs = (int)inputs;
        boardManager.ShowOnlyRequiredPegs();
    }

    // Function called by the options checkbox to toggle the use of minimax with the AI
    public void ToggleMinimax(System.Boolean useMinimax)
    {
        guessWithMinimax = useMinimax;
    }

    public void StartCodemakerRoutine()
    {
        if (!inRoutine)
        {
            inRoutine = true;
            StartCoroutine(CodemakerRoutine());
        }
    }

    public void StartCodebreakerRoutine()
    {
        if (!inRoutine)
        {
            inRoutine = true;
            StartCoroutine(CodebreakerRoutine());
        }
    }

    public void StartEndingRoutine(bool actualCodeFound)
    {
        StartCoroutine(EndingRoutine(actualCodeFound));
    }

    // Routine that handles the codemaker state chosen by the player
    public IEnumerator CodemakerRoutine()
    {
        // Initialise game type
        gametype = GameType.CODEMAKER;

        // Transition to codemaker humanoid
        cameraController.GoToCodemaker();

        // Play audio event during transition
        audioManager.PlayCameraSwooshShortEvent();

        // Fade out menu during transition
        StartCoroutine(uiManager.FadeOutMenu());

        // Prepare the board based on the game type
        boardManager.InitialiseBoard(gametype);

        // Wait until the user finished generating the actual code
        yield return new WaitUntil(() => actualCode != null);

        // Disable further player input after actual code is generated
        boardManager.DisablePlayerInput();

        // Look up at AI humanoid
        yield return new WaitForSeconds(cameraController.CodemakerLookUp());

        // Make AI start the guessing process
        StartCoroutine(codebreakerAI.StartGuessing(actualCode, boardManager));
    }

    // Routine that handles the codebreaker state chosen by the player
    public IEnumerator CodebreakerRoutine()
    {
        // Initialise game type
        gametype = GameType.CODEBREAKER;

        // Transition to codebreaker humanoid
        cameraController.GoToCodebreaker();

        // Play audio event during transition
        audioManager.PlayCameraSwooshEvent();

        // Fade out menu during transition
        StartCoroutine(uiManager.FadeOutMenu());

        // Generate random code
        actualCode = CodeGenerator.GenerateRandomCode();

        // Prepare the board based on the game type
        boardManager.InitialiseBoard(gametype);

        // Wait for 5 seocnds until the player fully transitions to the codebreaker humanoid
        yield return new WaitForSeconds(5);

        // Instantiate 3D pegs in the codemaker's hidden row based on the actual code
        boardManager.FillCodemakerRow(actualCode);
    }

    private IEnumerator EndingRoutine(bool actualCodeFound)
    {
        if(gametype == GameType.CODEBREAKER)
        {
            // Make codebreaker look up to codemaker humanoid
            cameraController.CodebreakerLookUp();

            if (actualCodeFound)
                // If the player guessed the actual code, the codemaker claps
                humanoidController.MakeCodemakerClap();
            else
                // If the player failed to guess the actual code, the codemaker celebrates
                humanoidController.MakeCodemakerCelebrate();
        }

        if (gametype == GameType.CODEMAKER)
        {
            if (actualCodeFound)
                // If the AI guessed the code, the AI humanoid celebrates
                humanoidController.MakeCodebreakerCelebrate();
            else
                // If the AI failed to guess the code, the AI humanoid claps
                humanoidController.MakeCodebreakerClap();
        }

        // Wait for animations to end and for player to observe the board
        yield return new WaitForSeconds(5);

        // transition back to initial state
        cameraController.GoToStartingPosition();

        // Fade in main menu
        StartCoroutine(uiManager.FadeInMenu());

        // Reset controller for next gamemode
        ResetControllerProperties();
    }

    private void ResetControllerProperties()
    {
        actualCode = null;
        currentGuess = 0;
        inRoutine = false;
    }
}
