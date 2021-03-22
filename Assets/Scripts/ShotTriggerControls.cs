﻿using Photon.Pun;
using UnityEngine;

public class ShotTriggerControls : MonoBehaviourPunCallbacks
{
    public int shootingPower;
    private Rigidbody2D rb;

    [PunRPC]
    void SendShotParameters()
    {

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.GetComponent<PhotonView>().IsMine) return;
            Debug.Log("Player trigger");
            PlayerDuelZoneControls player = other.GetComponent<PlayerDuelZoneControls>();
            player.LessPlayerHealth(shootingPower);
            player.ChangePlayerHealthBar();
            Debug.Log("Player health: " + player.GetPlayerHealth());
            if (player.GetPlayerHealth() <= 0) PhotonNetwork.LeaveRoom();
            Destroy(gameObject);
        }
    }
}
