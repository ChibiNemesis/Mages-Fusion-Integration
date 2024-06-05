using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] NetworkObject PlayerPrefab;
    [SerializeField] NetworkObject LeftHandPrefab;
    [SerializeField] NetworkObject RightHandPrefab;

    [Networked, Capacity(20)] private NetworkDictionary<PlayerRef, Player> Players => default;

    public void PlayerJoined(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            NetworkObject PlayerObject = Runner.Spawn(PlayerPrefab, Vector2.up, Quaternion.identity, player);
            //Spawn hands here
            Players.Add(player, PlayerObject.GetComponent<Player>());
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (!HasStateAuthority)
            return;
        if(Players.TryGet(player, out Player PlayerBehaviour))
        {
            Players.Remove(player);
            Runner.Despawn(PlayerBehaviour.Object);
            //Despawn hands
        }
    }
}
