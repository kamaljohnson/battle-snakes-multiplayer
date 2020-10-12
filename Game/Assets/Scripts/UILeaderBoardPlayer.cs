using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILeaderBoardPlayer : MonoBehaviour
{
    Player player;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text score;

    public void InitPlayerLeaderboardUnit(Player _player)
    {
        player = _player;
        playerName.text = player.name;
        score.text = "0";
    }
}
