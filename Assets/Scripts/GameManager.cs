using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    // constant values used by this script
    private const string MAIN_SCENE_NAME = "Main";
    private const int COUNTDOWN_TIME = 5;                       // number of seconds to countdown

    // serialized fields available in the editor for use in this script
    [Header("Data we don't want destroyed when scripts are swapped")]
    [SerializeField] List<GameObject> baseTeleportLocations;
    [SerializeField] GameObject[] necessaryGameObjects;
    [SerializeField] GameObject xrOrigin;                       // saving this separately as we need to teleport it

    [Header("Data for loading scenes and teleporting")]
    [SerializeField] NetworkManager networkManager;
    [SerializeField] TMP_Dropdown teleportSceneDropdown;
    [SerializeField] string[] teleportSceneNames;
    [SerializeField] TMP_Text teleportText;
    [SerializeField] GameObject countdownMenu;
    [SerializeField] TMP_Text countdownText;

    [Header("Menu Systems")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] GameObject rightHandUIController;
    [SerializeField] GameObject rightHandController;
    [SerializeField] GameObject leftHandController;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TMP_Dropdown movementSystem;
    [SerializeField] GameObject locomotionSystem;

    // private variables used by this script
    private AsyncOperation async;
    public string currentSceneName;                             // currently loaded scene name - making public for now so we can see it in the editor

    // countdown timer data for players
    private float countdownTimer;
    private bool countdownOn;

    // Photon Component
    private PhotonView pv;

    // Pause menu system variables
    private InputAction menuToggle;
    private bool menuActive;

    /// <summary>
    /// Start is called before the first frame update - sets up the basic variables and those to not get destroyed
    /// </summary>
    void Start()
    {
        // get the network components used by this script
        pv = GetComponent<PhotonView>();

        // for now grab the active scene name (should be main as that is the scene that is loaded first)
        currentSceneName = MAIN_SCENE_NAME;

        // make it so the game manager doesn't get destroyed when the scene moves
        DontDestroyOnLoad(this.gameObject);
        // game objects that should not get destoryed (XR Origin probably among others)
        for (int i = 0; i < necessaryGameObjects.Length; i++)
        {
            DontDestroyOnLoad(necessaryGameObjects[i]);
        }

        // set up the options menu toggle system
        var inputActionMap = inputActions.FindActionMap("XRI LeftHand Interaction");
        menuToggle = inputActionMap.FindAction("Menu Toggle");
        menuToggle.performed += ToggleMenu;
        menuToggle.Enable();

        // set up the teleport level options
        PopulateDropDownWithStrings(teleportSceneDropdown);

    } // end Start

    /// <summary>
    /// Used to togle the pause menu on/off
    /// </summary>
    /// <param name="context"></param>
    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (menuActive)
        {
            menuActive = false;
            pauseMenu.SetActive(false);
            if (movementSystem.value == 0)
            {
                rightHandUIController.SetActive(false);
            }
            //rightHandController.SetActive(true);
        }
        else
        {
            menuActive = true;
            pauseMenu.SetActive(true);
            rightHandUIController.SetActive(true);
            //rightHandController.SetActive(false);
        }
    }

    public void OnChangeMovement()
    {
        if(movementSystem.value==0)                                                                     //Continuous Movement
        {
            Debug.Log("Continuous");
            locomotionSystem.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
            rightHandUIController.SetActive(false);
        }
        else if(movementSystem.value==1)                                                                //Teleport Movement
        {
            Debug.Log("Teleport");
            locomotionSystem.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
            rightHandUIController.SetActive(true);
        }
    }
    /// <summary>
    /// Exits the game - used by option menu
    /// </summary>
    public void QuitGame()
    {
        // TODO: Add any clean up code here! Some classes may need to call OnApplicationQuit methods to clean up
        Application.Quit();

    } // end QuitGame

    /// <summary>
    /// Teleports to a puzzle after loading it and counting down
    /// </summary>
    public void TeleportToPuzzle()
    {
        // set up the teleport
        pv.RPC("LoadLevelSync", RpcTarget.All);

        // for debugging, adding the player id and information to the teleport text for now
        teleportText.text = "Press the button to start the next puzzle!\nActor ID: " + PhotonNetwork.LocalPlayer.ActorNumber;

    } // end TeleportToPuzzle

    /// <summary>
    /// Teleport back function to be used by the teleport objects
    /// </summary>
    public void TeleportBack()
    {
        // unload the puzzle level
        pv.RPC("UnloadLevelSync", RpcTarget.All);

    } // end TeleportBack

    [PunRPC]
    public void LoadLevelSync()
    {
        // Go to the option the player had selected on the menu
        StartCoroutine(LoadLevel(teleportSceneNames[teleportSceneDropdown.value]));

    } // end LoadLevelSync

    [PunRPC]
    public void UnloadLevelSync()
    {
        StartCoroutine(UnloadLevel());

    } // end UnloadLevelSync

    // Co-routine to load a level given the level name in string form
    IEnumerator LoadLevel(string levelName)
    {
        StartCountdownTimer();

        // store the current scene as the previous scene (we only teleport to one scene at a time)
        currentSceneName = levelName;

        Debug.Log("Attempting to load " + currentSceneName);

        // store the async information
        async = SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);

        // wait until the scene is loaded
        while (!async.isDone)
        {
            UpdateCountdownTimer();
            // Loading the scene asynchronusly, to avoid locking up the screen
            yield return null;
        }

        // make sure we are done counting down before moving scenes
        while (countdownOn)
        {
            UpdateCountdownTimer();
            yield return null;
        }

        // set the active scene to the new puzzle room
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));

        // teleport the player(s) to the start location of the puzzle scene for this systems player
        // (each puzzle scene should have 10 locations)
        Debug.Log("Actor ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        int playerTeleportPos = networkManager.getPlayerIDZeroBased();

        // Find the start location and tranport the player there
        GameObject startLocation = GameObject.Find("StartLocation" + playerTeleportPos);
        if (startLocation == null)
        {
            Debug.Log("No start location found for player with ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            xrOrigin.transform.position = startLocation.transform.position;
            xrOrigin.transform.rotation = startLocation.transform.rotation;
        }

        // prnt out the location
        Debug.Log(currentSceneName + " loaded. Teleporting!");

    } // end LoadLevel

    IEnumerator UnloadLevel()
    {
        StartCountdownTimer();

        // make sure we are done counting down before moving scenes
        while (countdownOn)
        {
            UpdateCountdownTimer();
            yield return null;
        }

        // unload the previously loaded scene so we don't have two on top of each other
        Scene loadedLevel = SceneManager.GetSceneByName(currentSceneName);

        // keep the async information so we can wait for the scene to unload
        async = null;

        // only unload the level if it is loaded and not the main scene
        if (loadedLevel.isLoaded)
        {
            async = SceneManager.UnloadSceneAsync(currentSceneName);
            Debug.Log("Attempting to unload " + currentSceneName);
        }

        // set the main scene to active again
        currentSceneName = MAIN_SCENE_NAME;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));

        // teleport the player(s) to their spot in the base hub
        int playerTeleportPos = networkManager.getPlayerIDZeroBased();

        GameObject baseLocation = baseTeleportLocations[playerTeleportPos];
        xrOrigin.transform.position = baseLocation.transform.position;
        xrOrigin.transform.rotation = baseLocation.transform.rotation;

        // now wait for the async operation to complete
        while ( (async != null) && !async.isDone)
        {
            yield return null;
        }

        Debug.Log("Level Unloaded");

    } // end UnloadLevel

    /// <summary>
    /// Sets up the countdown timer for the players
    /// </summary>
    private void StartCountdownTimer()
    {
        countdownMenu.SetActive(true);
        countdownTimer = COUNTDOWN_TIME;
        countdownOn = true;

    } // end StartCountdownTimer

    /// <summary>
    /// Updates the countdown timer and lets the system know it is time to teleport
    /// </summary>
    private void UpdateCountdownTimer()
    {
        //if (countdownOn)
        {
            countdownTimer -= Time.deltaTime;

            // update the text for the count down on this game
            countdownText.text = "Teleporting in " + (int)countdownTimer + " seconds!";

            if (countdownTimer  < 0)
            {
                // time to teleport - add teleport here or do we do it in the co-routine still.
                countdownOn = false;
                countdownMenu.SetActive(false);
            }
        }

    } //end UpdateCountdownTimer

    /// <summary>
    /// You can populate any dropdown with a list of strings with this method
    /// </summary>
    /// <param name="dropdown">The UI dropdown to populate</param>
    private void PopulateDropDownWithStrings(TMP_Dropdown dropdown)
    {

        // create a list to put into the options
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        // populate thi newOptions list
        for (int i = 0; i < teleportSceneNames.Length; i++)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(teleportSceneNames[i]));
        }

        // clear the option list and add the enum options
        dropdown.ClearOptions();
        dropdown.AddOptions(newOptions);

    } // end PopulateDropDownWithEnum

}
