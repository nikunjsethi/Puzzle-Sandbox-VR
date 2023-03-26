using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // constants that can be used by other classes
    public static string ROOM_NAME = "PuzzleHub";
    public static int MAX_NUM_PLAYERS = 10;

    // serialized fields for this script
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject joinRoomButton;
    [SerializeField] GameObject teleportMenu;
    [SerializeField] GameObject teleportButton;
    [SerializeField] TMP_Text statusBox;
    private PhotonView pv;
    // public variables used by other scripts
    public int numCubesToReplace = 0;
    public bool playerInLevel = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        ConnectToServer();
    }

    private void Update()
    {
        // if the number of cubes to place is less than 0, then all cubes are placed and make the teleport system active
        pv.RPC("ActiveTeleport", RpcTarget.AllBuffered);
        
    }

    [PunRPC]
    void ActiveTeleport()
    {
        if (pv.IsMine)
        {
            if (playerInLevel && (numCubesToReplace <= 0))
            {
                teleportMenu.SetActive(true);
                teleportButton.SetActive(true);
            }
        }
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
}
