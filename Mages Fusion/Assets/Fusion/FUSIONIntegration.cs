using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Linq;
using MAGES.Interaction.Interactables;
using MAGES.Networking;
using MAGES.SceneGraph;
using MAGES.Utilities;
using Fusion;
using MAGES;
using Fusion.Sockets;
using System;

namespace MAGES.Networking
{
    public class FUSIONIntegration : SimulationBehaviour, IMAGESNetworkIntegration, INetworkRunnerCallbacks //or NetworkBehaviour
    {
        private bool isConnectedToServer;
        private List<SessionInfo> allRoomsInfo;
        private MAGESNetworking networking;
        private FusionMessageHandler networkMessageHandler;
        private short? returnCode = null;

        public FusionMessageHandler NetworkMessageHandler
        {
            get => networkMessageHandler;
            set => networkMessageHandler = value;
        }

        public void AddQuestionSyncScript(GameObject questionPrefab)
        {
            throw new System.NotImplementedException();
            //if (questionPrefab)
            //{
            //    questionPrefab.AddComponent<SyncQuestionState>();
            //}
            //SyncQuestionState -> INetworkRunnerCallbacks
        }

        public IMAGESObjectSynchronization AddSyncTransform(GameObject gameObject)
        {
            throw new System.NotImplementedException();
            //return gameObject.GetOrAddComponent<SyncTransformPhoton>();
        }

        public bool CreateRoom(string roomName)
        {
            var result = Runner.StartGame(new StartGameArgs()
            {
                SessionName = roomName,
                PlayerCount = 20,
                IsOpen = true,
                IsVisible = true,
                GameMode = GameMode.Shared //Shared mode is similar to PUN
            }
            );

            return true;
        }

        public bool DestroyComponent(GameObject gameObject, string componentType)
        {
            NetworkObject NetObject = gameObject.GetComponent<NetworkObject>();
            if (NetObject)
            {
                networkMessageHandler.DestroyComponent(NetObject, componentType);

                return true;
            }
            return false;

        }

        public bool DestroyObject(GameObject gameObject)
        {
            NetworkObject NetObject = gameObject.GetComponent<NetworkObject>();

            if (!NetObject)
            {
                Debug.LogError("The object you are trying to destroy is null.");
                return false;
            }
            if(NetObject!= null && NetObject.HasStateAuthority)
            {
                Runner.Despawn(NetObject);
            }
            else if(NetObject != null)
            {
                //This may not be needed unless the object does not have state authority
                Debug.Log("add callback or do like with transfer ownership...");
            }
            else
            {
                Debug.LogError("The object you are trying to network-destroy is not a network object.");
                return false;
            }

            return false;
        }

        public void Disconnect()
        {
            isConnectedToServer = false;
            returnCode = null;
            //Runner.Disconnect();
        }

        public bool EstablishConnectionToMainServer(string args)
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetAvailableRooms()
        {
            if (!IsConnectedToServer())
            {
                return null;
            }

            return (from room in allRoomsInfo where room != null select room.Name).ToList();
        }

        public int GetConnectedUsersToCurrentRoom()
        {
            if (Runner.SessionInfo != null)
            {
                return Runner.SessionInfo.PlayerCount;
            }

            return -1;
        }

        public int GetConnectedUsersToRoom(string roomID)
        {
            SessionInfo roomInfo = null;
            foreach (var room in allRoomsInfo)
            {
                if (roomID == room.Name)
                {
                    roomInfo = room;
                }
            }

            if (roomInfo != null)
            {
                return roomInfo.PlayerCount;
            }

            return -1;
        }

        public string GetCurrentConnectedRoom()
        {
            if (Runner.SessionInfo != null)
            {
                return Runner.SessionInfo.Name;
            }

            return null;
        }

        public int GetNetworkID(GameObject networkObject)
        {
            if (networkObject == null)
            {
                return -1;
            }

            var view = networkObject.GetComponent<NetworkObject>();
            if (view == null)
            {
                return -1;
            }

            return (int)view.Id.Raw;
        }

        public int GetPing()
        {
            throw new System.NotImplementedException();
        }

        public int GetPrefabIDFromNetwork(GameObject prefab, out bool isUnique)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAuthority(GameObject networkObject)
        {
            var newtrowkView = networkObject.GetComponent<NetworkObject>();
            {
                Debug.LogError("Has authority called for object " + gameObject.name + " which is not a network object.");
            }

            return newtrowkView.HasStateAuthority; //equivalent to isMine from PUN
        }

        public void InitSpawnedObjectForNetwork(GameObject gameObject, bool syncTransform)
        {
            throw new System.NotImplementedException();
        }

        public bool IsConnectedToServer()
        {
            throw new System.NotImplementedException();
        }

        public bool JoinRoom(string roomName)
        {
            Runner.JoinSessionLobby(new SessionLobby(), roomName); //hmm
            return true;
        }

        public GameObject LinkNetworkObject(GameObject remotePrefab, GameObject localPrefab)
        {
            if (remotePrefab.name != localPrefab.name)
            {
                return null;
            }

            var remotePview = remotePrefab.GetComponent<NetworkObject>();
            var viewID = remotePview.Id;
            var newPhotonView = localPrefab.GetOrAddComponent<NetworkObject>();
            //PhotonNetwork.LocalCleanPhotonView(remotePview);

            remotePrefab.SetActive(false);
            //PhotonNetwork.PrefabPool.Destroy(remotePrefab);

            Destroy(remotePrefab);
            //newPhotonView.Id = viewID;
            localPrefab.GetOrAddComponent<SyncTransform>().Initialise();

            return localPrefab;

            throw new System.NotImplementedException();
        }

        public void OnConnectedToServer(NetworkRunner runner) //OnConnectedToMaster(PUN)
        {
            isConnectedToServer = true;
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.LogError("Connection to Remote Address: " + remoteAddress.ToString()+" failed"
                            +"Reason: "+reason.ToString()
                );
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            throw new NotImplementedException();
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            throw new NotImplementedException();
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            throw new NotImplementedException();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player: " + player.PlayerId + " has joined");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player: " + player.PlayerId + " has left");
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
            throw new NotImplementedException();
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            throw new NotImplementedException();
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            allRoomsInfo = sessionList;
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            throw new NotImplementedException();
        }

        public void OnStartup()
        {
            Hub.Instance.Get<NetworkingModule>().NetworkIdType = typeof(NetworkObject); //Fusion
            networking = Hub.Instance.Get<MAGESNetworking>();
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            throw new NotImplementedException();
        }

        public void RequestAuthority(GameObject networkObject)
        {
            var networkComponent = networkObject.GetComponent<NetworkObject>();
            if (networkComponent != null)
            {
                networkComponent.RequestStateAuthority();
            }
            else
            {
                Debug.LogError($"Network Request Ownership called for object {gameObject.name}, which does not have photonView component.");
            }
        }

        public void RequestChangeState(byte changeState, string actionID)
        {
            if (NetworkMessageHandler == null)
            {
                var messageHandlerGo = GameObject.Find("FusionMessageHandler(Clone)");
                NetworkMessageHandler = messageHandlerGo.GetComponent<FusionMessageHandler>();
                if (!networkMessageHandler)
                {
                    return;
                }
            }

            networkMessageHandler.RequestStateChange(changeState, actionID);
        }

        public GameObject SpawnObject(GameObject prefab, bool isUnique = true)
        {
            if (prefab == null)
            {
                Debug.LogError("Network spawn called with null prefab.");
                return null;
            }

            GameObject spawnedObject = Instantiate(prefab);// TBC

            return spawnedObject;
        }

        public short? ValidateClientNetworkingSetup()
        {
            return returnCode;
        }
    }
}