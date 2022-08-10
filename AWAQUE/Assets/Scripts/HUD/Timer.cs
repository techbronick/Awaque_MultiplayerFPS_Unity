using System;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text ui_Timer;

    private void Update()
    {
        if ((Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["GameStarting"])) == true)
        {
            string message = "Game is starting";
            ui_Timer.text = $"{message}";
        }
        else
        {
            string minutes = (Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["CurrentMatchTime"]) / 60).ToString("00");
            string seconds = (Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["CurrentMatchTime"]) % 60).ToString("00");
            ui_Timer.text = $"{minutes}:{seconds}";
        }

        if ((Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["GameEnding"])) == true)
        {
            string message = "Game Over";
            ui_Timer.text = $"{message}";
        }

    }
}
