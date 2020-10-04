using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{

    public static GameUIManager instance;

    [Header("Leaderboard")]
    [SerializeField] Transform UIPlayerLeaderboardParent;
    [SerializeField] GameObject UIPlayerLeaderboardUnitPrefab;

    public void Start()
    {
        instance = this;
        InitLeaderboard();
    }

    public void InitLeaderboard()
    {
        foreach (var _player in FindObjectsOfType<Player>())
        {
            Debug.Log("_player: " + _player.name);
            AddPlayerToLeaderbaord(_player);
        }
    }

    public void AddPlayerToLeaderbaord(Player _player)
    {
        GameObject newUIPlayerLeaderboardUnit = Instantiate(UIPlayerLeaderboardUnitPrefab, UIPlayerLeaderboardParent);
        newUIPlayerLeaderboardUnit.GetComponent<UILeaderBoardPlayer>().InitPlayerLeaderboardUnit(_player);
        newUIPlayerLeaderboardUnit.transform.SetSiblingIndex(_player.playerIndex - 1);
    }
}
