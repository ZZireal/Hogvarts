using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class DuelClubManager : MonoBehaviourPunCallbacks
{
    public MultiplayerConnectionControls multiplayerConnectionControls;
    public InternetConnectionControls internetConnectionControls;
    public Text logText;
    public GameObject createDuelZoneButton;
    public GameObject joinDuelZoneButton;
    public void LoadMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //automatically sync scene between players
        PhotonNetwork.NickName = FirebaseAuth.DefaultInstance.CurrentUser.Email.ToString();
        PhotonNetwork.GameVersion = "1";
        Log("Добро пожаловать в дуэльный клуб, " + PhotonNetwork.NickName + "!");
        Log("\nПожалуйста, подождите, пока обустраивается зона для дуэли...");

        PhotonNetwork.ConnectUsingSettings(); //connect to master with these settings for matchmaking

        StartCoroutine(ControlInternetConnectionl());
        Debug.Log("start control internet conenction");
    }

    public override void OnConnectedToMaster()
    {
        Log("\nЗона дуэли обсутроена!");

        createDuelZoneButton.SetActive(true);
        StartCoroutine(ToggleIsRoomToConnectReady());

        Log("\nВы можете создать свою дуэль и дождаться соперника или присоединиться к уже существующей дуэли!");

    }

    private IEnumerator ControlInternetConnectionl()
    {
        if (internetConnectionControls.IsInternetConnection())
        {
            yield return new WaitForSeconds(2f);
            StartCoroutine(ControlInternetConnectionl());
        }
        else
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainMenu");
            internetConnectionControls.ShowInternetConnectionErrorToast();
        }
    }

    private IEnumerator ToggleIsRoomToConnectReady()
    {
        if (PhotonNetwork.CountOfPlayersInRooms % 2 != 0) Debug.Log("There is room!");
        if (PhotonNetwork.CountOfPlayersInRooms % 2 == 0) Debug.Log("There is no room!");
        Debug.Log(PhotonNetwork.CountOfPlayersInRooms);

        joinDuelZoneButton.SetActive(PhotonNetwork.CountOfPlayersInRooms % 2 != 0);
        yield return new WaitForSeconds(1f);
        StartCoroutine(ToggleIsRoomToConnectReady());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Ошибка при попытке создания дуэли! Пожалуйста, попробуйте еще раз");
        multiplayerConnectionControls.ShowMultiplayerConnectionErrorToast("Ошибка при попытке создания дуэли! Пожалуйста, попробуйте еще раз.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Ошибка при попытке присоединиться к дуэли! Пожалуйста, попробуйте еще раз");
        multiplayerConnectionControls.ShowMultiplayerConnectionErrorToast("Ошибка при попытке присоединиться к дуэли! Пожалуйста, попробуйте еще раз.");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Ошибка при попытке присоединиться к дуэли! Пожалуйста, попробуйте еще раз");
        multiplayerConnectionControls.ShowMultiplayerConnectionErrorToast("Ошибка при попытке присоединиться к дуэли! Пожалуйста, попробуйте еще раз.");
    }

    public void CreateDuelZone()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }

    public void JoinDuelZone()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Log("\nВы заходите в зону дуэли!");
        PhotonNetwork.LoadLevel("DuelZone");
    }

    private void Log(string message)
    {
        Debug.Log(message);
        logText.text += "\n";
        logText.text += message;
    }
}
