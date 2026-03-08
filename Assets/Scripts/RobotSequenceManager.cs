using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;


public class RobotSequenceManager : MonoBehaviour
{
    public enum GamePhase { StartMenu, IntroVideo, FreefallAssembly, Parachute, EndCredits }
    public GamePhase currentPhase;

    [Header("EVE Sockets")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor neckSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor backpackSocket;

    [Header("Alle Robot Onderdelen")]
    public GameObject headObj;
    public GameObject backpackObj;
    public GameObject bodyObj; // De Body is nu ook een onderdeel!

    [Header("Phase Events")]
    public UnityEvent OnStartMenuEnter;
    public UnityEvent OnIntroVideoEnter;
    public UnityEvent OnFreefallAssemblyEnter;
    public UnityEvent OnParachuteEnter;
    public UnityEvent OnEndCreditsEnter;

    private Vector3 headStartPos, backpackStartPos, bodyStartPos;

    void Start()
    {
        // Bewaar de allereerste startposities voor de reset later
        headStartPos = headObj.transform.position;
        backpackStartPos = backpackObj.transform.position;
        bodyStartPos = bodyObj.transform.position;

        SwitchPhase(GamePhase.FreefallAssembly);
    }

    public void SwitchPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        Debug.Log("Game Phase is nu: " + currentPhase.ToString());

        switch (currentPhase)
        {
            case GamePhase.StartMenu:
                ResetRobotForNewRound(); // Zorg dat EVE weer in stukken ligt!
                OnStartMenuEnter.Invoke();
                break;

            case GamePhase.IntroVideo:
                OnIntroVideoEnter.Invoke();
                StartCoroutine(SkipIntroForNow());
                break;

            case GamePhase.FreefallAssembly:
                OnFreefallAssemblyEnter.Invoke();
                
                // Zet het vallen AAN voor alle 3 de onderdelen
                SetScriptsActive(headObj, true);
                SetScriptsActive(backpackObj, true);
                SetScriptsActive(bodyObj, true);
                break;

            case GamePhase.Parachute:
                // HIER STOPPEN WE HET WOBBELEN VAN DE GEHELE EVE!
                ModularWobble bodyWobble = bodyObj.GetComponent<ModularWobble>();
                if (bodyWobble != null) bodyWobble.enabled = false;

                OnParachuteEnter.Invoke();
                StartCoroutine(TransitionToCreditsDelay(5f));
                break;

            case GamePhase.EndCredits:
                OnEndCreditsEnter.Invoke();
                StartCoroutine(BackToMenuDelay(4f)); // Gaat na credits terug naar StartMenu (dus reset!)
                break;
        }
    }

    public void CheckAssemblyProgress()
    {
        if (currentPhase != GamePhase.FreefallAssembly) return;

        if (neckSocket.hasSelection && backpackSocket.hasSelection)
        {
            Debug.Log("EVE is compleet samengebouwd!");
            SwitchPhase(GamePhase.Parachute);
        }
    }

    // --- DE MAGISCHE RESET FUNCTIE VOOR DE LOOP ---
    private void ResetRobotForNewRound()
    {
        // Haal de items UIT de sockets als ze daar in zitten
        if (neckSocket.hasSelection) neckSocket.interactionManager.CancelInteractorSelection((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)neckSocket);
        if (backpackSocket.hasSelection) backpackSocket.interactionManager.CancelInteractorSelection((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)backpackSocket);

        // Reset elk object: ontkoppel ze, zet scripts aan, en zet ze op hun startplek
        ResetSinglePart(headObj, headStartPos);
        ResetSinglePart(backpackObj, backpackStartPos);
        ResetSinglePart(bodyObj, bodyStartPos);
    }

    private void ResetSinglePart(GameObject part, Vector3 startPos)
    {
        // 1. Maak hem weer los (geen child meer van de body/socket)
        part.transform.SetParent(null); 
        
        // 2. Zet hem terug in de lucht
        part.transform.position = startPos; 

        // 3. Zet fysica weer aan
        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // 4. Zet vastpakken weer aan
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = part.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) grab.enabled = true;

        // 5. Reset de interne kill-switches in onze scripts
        AerodynamicPart aero = part.GetComponent<AerodynamicPart>();
        if (aero != null)
        {
            aero.enabled = true;
            aero.hasBeenGrabbed = false;
        }

        ModularWobble wobble = part.GetComponent<ModularWobble>();
        if (wobble != null)
        {
            wobble.enabled = true;
            wobble.ResetForNewRound(); // Roept de functie aan die we in de vorige stap maakten
        }
    }

    private void SetScriptsActive(GameObject part, bool state)
    {
        AerodynamicPart aero = part.GetComponent<AerodynamicPart>();
        if (aero != null) aero.SetFreefallState(state);
    }

    private IEnumerator SkipIntroForNow() { yield return new WaitForSeconds(1f); SwitchPhase(GamePhase.FreefallAssembly); }
    private IEnumerator TransitionToCreditsDelay(float delay) { yield return new WaitForSeconds(delay); SwitchPhase(GamePhase.EndCredits); }
    private IEnumerator BackToMenuDelay(float delay) { yield return new WaitForSeconds(delay); SwitchPhase(GamePhase.StartMenu); }
}