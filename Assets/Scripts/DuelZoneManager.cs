using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DuelZoneManager : MonoBehaviourPunCallbacks
{
    public Text winnerText;
    public GameObject playerPrefab;

    void Start()
    {
        Vector3 startPosition = new Vector3(Random.Range(-2f, 2f), -0.7f, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, startPosition, Quaternion.identity);
    }

    public void LeaveDuelZone()
    {
        Debug.Log("Leave duel zone was clicked!");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() //when current player was left room
    {
        SceneManager.LoadScene("DuelClub");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        winnerText.text = "Вы победили игрока " + otherPlayer.NickName + "!";
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
}
