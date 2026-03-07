using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class RobotSequenceManager : MonoBehaviour
{
    // 5 fases of the game
    public enum GamePhase 
    {
        StartMenu,
        IntroVideo,
        FreefallAssembly,
        Parachute,
        EndCredits
    }

    [Header("Current State")]
    public GamePhase currentPhase;

    [Header("EVE Assembly Requirements")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor neckSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor backpackSocket;

    [Header("Phase Events")]
    public UnityEvent OnStartMenuEnter;
    public UnityEvent OnIntroVideoEnter;
    public UnityEvent OnFreefallAssemblyEnter;
    public UnityEvent OnParachuteEnter;
    public UnityEvent OnEndCreditsEnter;

    private void Start()
    {
        // If the game starts; force teh startmenu
        // SwitchPhase(GamePhase.StartMenu);
        SwitchPhase(GamePhase.FreefallAssembly);
    }

    // Logic switch fase
    public void SwitchPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        Debug.Log("Game Phase is nu: " + currentPhase.ToString());

       
        switch (currentPhase)
        {
            case GamePhase.StartMenu:
                OnStartMenuEnter.Invoke();
                break;

            case GamePhase.IntroVideo:
                OnIntroVideoEnter.Invoke();
                // Skip fit after 1 sec.
                StartCoroutine(SkipIntroForNow());
                break;

            case GamePhase.FreefallAssembly:
                OnFreefallAssemblyEnter.Invoke();
                break;

            case GamePhase.Parachute:
                OnParachuteEnter.Invoke();
                // Simulate parachute and then skip
                StartCoroutine(TransitionToCreditsDelay(2f));
                break;

            case GamePhase.EndCredits:
                OnEndCreditsEnter.Invoke();
                // Show credits; and go back to start.
                StartCoroutine(BackToMenuDelay(4f));
                break;
        }
    }

    // --- Public triggers (to use via buttons or sockets) ---

    // This is for the startgame button
    public void TriggerStartGame()
    {
        if (currentPhase == GamePhase.StartMenu)
        {
            SwitchPhase(GamePhase.IntroVideo);
        }
    }

    // This function is called if an object clicks into a socket
    public void CheckAssemblyProgress()
    {
        // Check phase
        if (currentPhase != GamePhase.FreefallAssembly) return;

        // Check if both sockets are filled
        bool isNeckFull = neckSocket.hasSelection;
        bool isBackpackFull = backpackSocket.hasSelection;

        if (isNeckFull && isBackpackFull)
        {
            Debug.Log("EVE is compleet! Start parachute fase.");
            SwitchPhase(GamePhase.Parachute);
        }
    }

    // --- Temporary Timers (Coroutines) ---

    private IEnumerator SkipIntroForNow()
    {
        yield return new WaitForSeconds(1f); // Korte pauze
        SwitchPhase(GamePhase.FreefallAssembly);
    }

    private IEnumerator TransitionToCreditsDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchPhase(GamePhase.EndCredits);
    }

    private IEnumerator BackToMenuDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchPhase(GamePhase.StartMenu);
    }
}