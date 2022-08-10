using TMPro;
using UnityEngine;
using Photon.Realtime;
using System;

public class ScoreBoardItem : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public void Initialize(Player player)
    {
        usernameText.text = player.NickName;
    }

    public void UpdateScore(Player player)
    {
        if (player.CustomProperties["Kills"] != null)
            killsText.text = Convert.ToString(player.CustomProperties["Kills"]);

        if (player.CustomProperties["Deaths"] != null)
            deathsText.text = Convert.ToString(player.CustomProperties["Deaths"]);
    }
}
