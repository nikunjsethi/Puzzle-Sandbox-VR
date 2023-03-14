using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // constant values used by this script
    private const string MAIN_SCENE_NAME = "Main";

    // serialized fields available in the editor for use in this script
    [Header("Data we don't want destroyed when scripts are swapped")]
    [SerializeField] List<GameObject> baseTeleportLocations;
    [SerializeField] GameObject[] necessaryGameObjects;
    [SerializeField] GameObject xrOrigin;                       // saving this separately as we need to teleport it

    [Header("Data for loading scenes and teleporting")]
    [SerializeField] string[] teleportSceneNames;
    [SerializeField] TMP_Text teleportText;

    // private variables used by this script
    private AsyncOperation async;
    public string currentSceneName;                             // currently loaded scene name - making public for now so we can see it in the editor

    /// <summary>
    /// Start is called before the first frame update - sets up the basic variables and those to not get destroyed
    /// </summary>
    void Start()
    {
        // for now grab the active scene name (should be main as that is the scene that is loaded first)
        currentSceneName = MAIN_SCENE_NAME;

        // make it so the game manager doesn't get destroyed when the scene moves
        DontDestroyOnLoad(this.gameObject);

        // game objects that should not get destoryed (XR Origin probably among others)
        for (int i = 0; i < necessaryGameObjects.Length; i++)
        {
            DontDestroyOnLoad(necessaryGameObjects[i]);
        }

    } // end Start

    public void TeleportToPuzzle()
    {
        // set up the teleport
        // TODO: This only goes to the first scene in the array, need to set it up that another button sets a variable to the correct location
        StartCoroutine(LoadLevel(teleportSceneNames[0]));

        // for debugging, adding the player id and information to the teleport text for now
        teleportText.text = "Press the button to start the next puzzle!\nActor ID: " + PhotonNetwork.LocalPlayer.ActorNumber;

        // debug log
        Debug.Log("Loading level TestTeleporter");

    } // end TeleportToPuzzle

    public void TeleportBack()
    {
        // unload the puzzle level
        StartCoroutine(UnloadLevel());

    } // TeleportBack

    // Co-routine to load a level given the level name in string form
    IEnumerator LoadLevel(string levelName)
    {
        // store the current scene as the previous scene (we only teleport to one scene at a time)
        currentSceneName = levelName;

        Debug.Log("Attempting to load " + currentSceneName);

        // store the async information
        async = SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);

        // wait until the scene is loaded
        while (!async.isDone)
        {
            // Loading the scene asynchronusly, to avoid locking up the screen
            yield return null;
        }

        // set the active scene to the new puzzle room
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));

        // teleport the player(s) to the start location of the puzzle scene for this systems player
        // (each puzzle scene should have 10 locations)
        Debug.Log("Actor ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        int playerTeleportPos = (PhotonNetwork.LocalPlayer.ActorNumber - 1);

        // keep the locations in the range of the available teleport locations
        if ((playerTeleportPos < 0) || (playerTeleportPos >= NetworkManager.MAX_NUM_PLAYERS))
        {
            playerTeleportPos = 0;
        }

        // TODO: Need to figure out rotation (or use teleport pads instead of empty objects)
        GameObject startLocation = GameObject.Find("StartLocation" + playerTeleportPos);
        if (startLocation == null)
        {
            Debug.Log("No start location found for player with ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            xrOrigin.transform.position = startLocation.transform.position;
        }

        // prnt out the location
        Debug.Log(currentSceneName + " loaded. Teleporting!");

    } // end LoadLevel

    IEnumerator UnloadLevel()
    {
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
        int playerTeleportPos = (PhotonNetwork.LocalPlayer.ActorNumber - 1);

        // keep the locations in the range of the available teleport locations
        if ((playerTeleportPos < 0) || (playerTeleportPos >= NetworkManager.MAX_NUM_PLAYERS))
        {
            playerTeleportPos = 0;
        }

        GameObject baseLocation = baseTeleportLocations[playerTeleportPos];
        xrOrigin.transform.position = baseLocation.transform.position;

        // now wait for the async operation to complete
        while ( (async != null) && !async.isDone)
        {
            yield return null;
        }

        Debug.Log("Level Unloaded");

    } // end UnloadLevel
}
