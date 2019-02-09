/*
 * transfers the info that the player put in the lobby into the
 * actual game (note that the lobby and game are diffrent scenes)
 * 
 */

using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerData player = gamePlayer.GetComponent<PlayerData>();
        player.Cname = lobby.name;
        player.spriteColor = lobby.playerColor;

    }
}
