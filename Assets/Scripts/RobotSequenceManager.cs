using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class RobotSequenceManager : MonoBehaviour
{
    public enum GamePhase { StartMenu, IntroVideo, FreefallAssembly, Parachute, EndCredits }
    public GamePhase currentPhase;

    [Header("EVE Sockets")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor neckSocket;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor backpackSocket;
    public bool isHeadAttached = false;
    public bool isBackpackAttached = false;

    [Header("All Robot Objects")]
    public GameObject headObj;
    public GameObject backpackObj;
    public GameObject bodyObj; // Body is main obj

    [Header("Phase Events")]
    public UnityEvent OnStartMenuEnter;
    public UnityEvent OnIntroVideoEnter;
    public UnityEvent OnFreefallAssemblyEnter;
    public UnityEvent OnParachuteEnter;
    public UnityEvent OnEndCreditsEnter;

    [Header("Player Setup")]
    public Transform xrOrigin; // XR Origin (whole rig)
    public Transform couchAnchor; // Anchor of where it needs to restart

    [Header("Passthrough & Skybox Setup")]
    public Camera mainVRCamera; // Main Cam
    
    private ARCameraBackground arBackground;
    private Vector3 headStartPos, backpackStartPos, bodyStartPos;

    void Start()
    {
        // Store the first positions for restart purposes
        headStartPos = headObj.transform.position;
        backpackStartPos = backpackObj.transform.position;
        bodyStartPos = bodyObj.transform.position;

        SwitchPhase(GamePhase.StartMenu);
    }

    public void SwitchPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        Debug.Log("Game Phase is now: " + currentPhase.ToString());

        switch (currentPhase)
        {
            case GamePhase.StartMenu:
                ResetRobotForNewRound(); // Restore the robot
                // SetPassthroughMode(true);
                OnStartMenuEnter.Invoke();
                break;

            case GamePhase.IntroVideo:
                ResetRobotForNewRound();
                OnIntroVideoEnter.Invoke();
                StartCoroutine(SkipIntroForNow());
                break;

            case GamePhase.FreefallAssembly:
                OnFreefallAssemblyEnter.Invoke();
                // SetPassthroughMode(false);
                
                // Set fall ON for all three objects of the robot
                SetScriptsActive(headObj, true);
                SetScriptsActive(backpackObj, true);
                SetScriptsActive(bodyObj, true);
                break;

            case GamePhase.Parachute:
                // Stop the wobbling of the whole bot
                ModularWobble bodyWobble = bodyObj.GetComponent<ModularWobble>();
                if (bodyWobble != null) bodyWobble.enabled = false;

                OnParachuteEnter.Invoke();
                StartCoroutine(TransitionToCreditsDelay(5f));
                break;

            case GamePhase.EndCredits:
                OnEndCreditsEnter.Invoke();
                StartCoroutine(BackToMenuDelay(4f)); // From credits; go to startmenu
                break;
        }
    }

    public void CheckAssemblyProgress()
{
    if (currentPhase != GamePhase.FreefallAssembly) return;

    // Check with booleans
    if (isHeadAttached && isBackpackAttached)
    {
        Debug.Log("Robot is complete according to the flags!");
        SwitchPhase(GamePhase.Parachute);
    }
}

    // --- Reset function for the loop ---
    public void ResetRobotForNewRound()
    {
        // Retrieve items out of the sockets, if they are there
        if (neckSocket.hasSelection) neckSocket.interactionManager.CancelInteractorSelection((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)neckSocket);
        if (backpackSocket.hasSelection) backpackSocket.interactionManager.CancelInteractorSelection((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)backpackSocket);

        // Reset every object; disconnect them, activate scripts and put them back at their starting place
        ResetSinglePart(headObj, headStartPos);
        ResetSinglePart(backpackObj, backpackStartPos);
        ResetSinglePart(bodyObj, bodyStartPos);
    }

    private void ResetSinglePart(GameObject part, Vector3 startPos)
    {
        part.transform.SetParent(null); 
        part.transform.position = startPos; 

        Rigidbody rb = part.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.isKinematic = true; // FIX: to keep control this must be kinematic!
            rb.linearVelocity = Vector3.zero; // stop all speed
            rb.angularVelocity = Vector3.zero; // stop rotation
        }

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = part.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) grab.enabled = true;

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
            wobble.ResetForNewRound();
        }
    }

    private void SetScriptsActive(GameObject part, bool state)
    {
        AerodynamicPart aero = part.GetComponent<AerodynamicPart>();
        if (aero != null) aero.SetFreefallState(state);
    }

    private IEnumerator SkipIntroForNow() { yield return new WaitForSeconds(24f); SwitchPhase(GamePhase.FreefallAssembly); }
    private IEnumerator TransitionToCreditsDelay(float delay) { yield return new WaitForSeconds(delay); SwitchPhase(GamePhase.EndCredits); }
    private IEnumerator BackToMenuDelay(float delay) { yield return new WaitForSeconds(delay); SwitchPhase(GamePhase.StartMenu); }

    public void ResetPlayerPosition()
{
    XROrigin originScript = xrOrigin.GetComponent<XROrigin>();
    
    if (originScript != null && couchAnchor != null)
    {
        // Function exactly moves the rig, so that the cam is exactly on the pos of the anchor 
        // and looks from the anchor
        originScript.MatchOriginUpCameraForward(couchAnchor.up, couchAnchor.forward);
        
        // extra check
        Vector3 offset = originScript.Camera.transform.position - xrOrigin.position;
        xrOrigin.position = couchAnchor.position - offset;

        Debug.Log("Player calibrated!");
    }
}
    public void TriggerStartGame()
    {
        // Go to intro video phase
        SwitchPhase(GamePhase.IntroVideo);
    }

    public void SetPassthroughMode(bool isActive)
{
    // Searching for AR Cam Background
   
    arBackground = mainVRCamera.GetComponent<ARCameraBackground>();

    if (isActive)
    {
        // 1. Make cam background transparant (Alpha = 0)
        mainVRCamera.clearFlags = CameraClearFlags.SolidColor;
        mainVRCamera.backgroundColor = new Color(0, 0, 0, 0); 
        
        // 2. SET AR Background ON
        if (arBackground != null) arBackground.enabled = true;
    }
    else
    {
        // 1. Back to Skybox
        mainVRCamera.clearFlags = CameraClearFlags.Skybox;
        
        // 2. Set AR BG off
        if (arBackground != null) arBackground.enabled = false;
    }
}
}