﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILobby : MonoBehaviour
{

    public static UILobby instance;

    [Header("Host Join")]
    [SerializeField] TMP_InputField joinMatchInput;
    [SerializeField] List<Selectable> lobbySelectables = new List<Selectable>();
    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] GameObject startGameButton;
    [SerializeField] Canvas searchCanvas;
    bool searching = false;

    [Header("Lobby")]
    [SerializeField] Transform UIPlayerParent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] TMP_Text matchIDText;
    [SerializeField] GameObject beginGameButton;
    [SerializeField] GameObject lobbyUI;

    GameObject localPlayerLobbyUI;

    void Start()
    {
        instance = this;
    }

    public void HostPublic()
    {
        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(true);
    }

    public void HostPrivate()
    {
        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(false);

        startGameButton.SetActive(true);    //only the host of the private party can start the game
    }

    public void HostSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;

            if (localPlayerLobbyUI != null) Destroy(localPlayerLobbyUI);
            localPlayerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
            beginGameButton.SetActive(true);
        }
        else
        {
            lobbySelectables.ForEach(x => x.interactable = true);
        }
    }

    public void Join()
    {
        lobbySelectables.ForEach(x => x.interactable = false);

        Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;

            if (localPlayerLobbyUI != null) Destroy(localPlayerLobbyUI);
            localPlayerLobbyUI = SpawnPlayerUIPrefab(Player.localPlayer);
            matchIDText.text = matchID;
        }
        else
        {
            lobbySelectables.ForEach(x => x.interactable = true);
        }
    }

    public void DisconnectGame()
    {
        if (localPlayerLobbyUI != null) Destroy(localPlayerLobbyUI);
        Player.localPlayer.DisconnectGame();

        lobbyCanvas.enabled = false;
        lobbySelectables.ForEach(x => x.interactable = true);
        beginGameButton.SetActive(false);
    }

    public GameObject SpawnPlayerUIPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);

        return newUIPlayer;
    }

    public void BeginGame()
    {
        Debug.Log("BeginGame()");
        Player.localPlayer.BeginGame();
    }

    public void SearchGame()
    {
        StartCoroutine(Searching());
    }

    public void CancelSearchGame()
    {
        searching = false;
    }

    public void SearchGameSuccess(bool success, string matchID)
    {
        searching = false;
        searchCanvas.enabled = false;
        if (success)
        {
            JoinSuccess(success, matchID);
        }
        else
        {
            //Host a new public game
            HostPublic();
        }
    }

    IEnumerator Searching()
    {
        Debug.Log("Searching...");
        searchCanvas.enabled = true;
        searching = true;

        float searchInterval = 1;
        float currentTime = 1;

        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = searchInterval;
                Player.localPlayer.SearchGame();
            }
            yield return null;
        }
        searchCanvas.enabled = false;
    }

    public void DisableLobbyUI()
    {
        lobbyUI.SetActive(false);
    }

}
