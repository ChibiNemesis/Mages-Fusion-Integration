namespace MAGES.Networking
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Fusion;

    public class PlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        private NetworkRunner _runner;
        private void Start()
        {
            _runner = GameObject.Find("Runner").GetComponent<NetworkRunner>();
        }

        public void PlayerJoined(PlayerRef player)
        {
            if (player == _runner.LocalPlayer)
            {
                Debug.Log("Local Player joined");
            }
            else
            {
                Debug.Log("Another Player Joined");
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == _runner.LocalPlayer)
            {
                Debug.Log("Local Player Left");
            }
            else
            {
                Debug.Log("Another Player Left");
            }
        }
    }
}