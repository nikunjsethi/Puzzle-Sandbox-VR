using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // constant values used by this script
    private const string MAIN_SCENE_NAME = "Main";

    // serialized fields available in the editor for use in this script
    [Header("Data we don't want destroyed when scripts are swapped")]
    [SerializeField] GameObject[] necessaryGameObjects;
    [SerializeField] GameObject xrOrigin;                       // saving this separately as we need to teleport it

    [Header("Mini-game scenes for loading")]
    [SerializeField] string[] teleportSceneNames;

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

        // teleport the player(s) to the start location of the puzzle scene (each puzzle scene should have this)
        // TODO: Need to figure out how to not teleport players on top of each other.
        GameObject startLocation = GameObject.Find("StartLocation");
        xrOrigin.transform.position = startLocation.transform.position;

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

        // teleport the player(s)
        GameObject baseLocation = GameObject.Find("BaseTeleportLocation");
        xrOrigin.transform.position = baseLocation.transform.position;

        // now wait for the async operation to complete
        while ( (async != null) && !async.isDone)
        {
            yield return null;
        }

        Debug.Log("Level Unloaded");

    } // end UnloadLevel
}
