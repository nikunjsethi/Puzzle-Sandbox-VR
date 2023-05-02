using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Oculus.Platform;
using TMPro;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // constants that can be used by other classes
    public static string ROOM_NAME = "PuzzleHub";
    public static int MAX_NUM_PLAYERS = 10;

    // serialized fields for this script
    //[SerializeField] public GameObject avatarEntity;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject joinRoomButton;
    [SerializeField] GameObject teleportMenu;
    [SerializeField] GameObject teleportButton;
    [SerializeField] TMP_Text statusBox;

    // public variables used by other scripts
    public int numCubesToReplace = 0;
    public bool playerInLevel = false;
    public ulong m_userId;

    // private variables
    private PhotonView pv;

    // may need to make this a singleton....
    // adding code to get the user id to share with the photon network, thanks to Meta Avatar SDK with Photon in unity
    // https://www.youtube.com/watch?v=PuKr3ZVloBo&t=13s
    //Singleton implementation
    /*private static NetworkManager m_instance;
    public static NetworkManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }*/

    void Start()
    {
        // adding code to get the user id to share with the photon network, thanks to Meta Avatar SDK with Photon in unity
        // https://www.youtube.com/watch?v=PuKr3ZVloBo&t=13s
        StartCoroutine(SetUserIdFromLoggedInUser());
        StartCoroutine(ConnectToPhotonOnceUserIsFound());

        // this needs to be moved to after pressing the button? - Moved to NetworkPlayerInstantiator.cs
        //StartCoroutine(InstantiateNetworkedAvatarOnceInRoom());

        // moved to the coroutine - ConnectToPhotonOnceUserIsFound
        //pv = GetComponent<PhotonView>();
        //ConnectToServer();
    }

    private void Update()
    {
        // if the number of cubes to place is less than 0, then all cubes are placed and make the teleport system active
        if (PhotonNetwork.IsMasterClient && pv.IsMine)
        {
            if (playerInLevel && (numCubesToReplace <= 0))
            {
                teleportMenu.SetActive(true);
                teleportButton.SetActive(true);
                pv.RPC("ActiveTeleport", RpcTarget.AllBuffered);
            }
        }        
        
    }

    [PunRPC]
    void ActiveTeleport()
    {
        teleportMenu.SetActive(true);
        teleportButton.SetActive(true);
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        //Debug.Log("Try connect to Server....");
        statusBox.text = "Connecting to server...";
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to Server.");
        new WaitForSeconds(0.5f);
        statusBox.text = "Connected to server!";
        base.OnConnectedToMaster();

        StartCoroutine(ShowJoinRoomButton());

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    /// <summary>
    /// Shows the button after a second so users have time to read the server message
    /// </summary>
    /// <returns>yields for a second before setting join room info up</returns>
    IEnumerator ShowJoinRoomButton()
    {
        yield return new WaitForSeconds(0.5f);
        statusBox.text = "Join " + ROOM_NAME + "!";
        joinRoomButton.SetActive(true);

    } // end ShowJoinRoomButton

    /// <summary>
    /// Hides the start menu after a second so it doesn't pop out of existence
    /// </summary>
    /// <returns></returns>
    IEnumerator HideStartMenu()
    {
        yield return new WaitForSeconds(1);
        startMenu.SetActive(false);
        joinRoomButton.SetActive(false);

    } // end HideStartMenu

    /// <summary>
    /// Used by the start method to allow player to join the room with other players
    /// </summary>
    public void JoinRoom()
    {
        StartCoroutine(HideStartMenu());
        new WaitForSeconds(.5f);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MAX_NUM_PLAYERS;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);

    } // end JoinRoom

    /// <summary>
    /// gets the current player id, used for teleporting
    /// </summary>
    /// <returns>The local player's id starting at 0 so it can be used in arrays</returns>
    public int getPlayerIDZeroBased()
    {
        // teleport the player(s) to their spot in the base hub
        int playerTeleportPos = (PhotonNetwork.LocalPlayer.ActorNumber - 1);

        // keep the locations in the range of the available teleport locations
        if ((playerTeleportPos < 0) || (playerTeleportPos >= NetworkManager.MAX_NUM_PLAYERS))
        {
            playerTeleportPos = 0;
        }

        return playerTeleportPos;

    } // end getPlayerIDZeroBased

    // adding code to get the user id to share with the photon network, thanks to Meta Avatar SDK with Photon in unity
    // https://www.youtube.com/watch?v=PuKr3ZVloBo&t=13s
    IEnumerator SetUserIdFromLoggedInUser()
    {
        if (OvrPlatformInit.status == OvrPlatformInitStatus.NotStarted)
        {
            OvrPlatformInit.InitializeOvrPlatform();
        }

        while (OvrPlatformInit.status != OvrPlatformInitStatus.Succeeded)
        {
            if (OvrPlatformInit.status == OvrPlatformInitStatus.Failed)
            {
                Debug.LogError("OVR Platform failed to initialise");
                statusBox.text = "OVR Platform failed to initialise";
                yield break;
            }
            yield return null;
        }

        Users.GetLoggedInUser().OnComplete(message =>
        {
            if (message.IsError)
            {
                Debug.LogError("Getting Logged in user error " + message.GetError());
            }
            else
            {
                m_userId = message.Data.ID;
            }
        });
    }

    // adding code to get the user id to share with the photon network, thanks to Meta Avatar SDK with Photon in unity
    // https://www.youtube.com/watch?v=PuKr3ZVloBo&t=13s
    IEnumerator ConnectToPhotonOnceUserIsFound()
    {
        while (m_userId == 0)
        {
            Debug.Log("Waiting for User id to be set before connecting to room");
            yield return null;
        }

        pv = GetComponent<PhotonView>();
        ConnectToServer(); 
        //ConnectToPhotonRoom();
    }

    // adding code to get the user id to share with the photon network, thanks to Meta Avatar SDK with Photon in unity
    // https://www.youtube.com/watch?v=PuKr3ZVloBo&t=13s - not needed in our setup?
    /*IEnumerator InstantiateNetworkedAvatarOnceInRoom()
    {
        while (PhotonNetwork.InRoom == false)
        {
            Debug.Log("Waiting to be in room before intantiating avatar");
            yield return null;
        }
        InstantiateNetworkedAvatar();
    }*/
}
