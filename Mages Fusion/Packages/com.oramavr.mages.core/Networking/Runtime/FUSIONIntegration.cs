namespace MAGES.Networking
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using MAGES.Interaction.Interactables;
    using MAGES.Networking;
    using MAGES.SceneGraph;
    using MAGES.Utilities;
    using Fusion;
    using MAGES;
    using Fusion.Sockets;
    using System;
    using System.Threading.Tasks;

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
            if (NetObject != null && NetObject.HasStateAuthority)
            {
                Runner.Despawn(NetObject);
            }
            else if (NetObject != null)
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
            Runner.Disconnect(Runner.ActivePlayers.FirstOrDefault()); //check this later
        }

        public bool EstablishConnectionToMainServer(string args)
        {
            Debug.Log("EstablishConnectionToMainServer called!");
            var result = EstablishConnectionToMainServerInner(args);
            //could add some error handling here

            return true;
        }

        private async Task<StartGameResult> EstablishConnectionToMainServerInner(string args)
        {
            var result = await Runner.JoinSessionLobby(SessionLobby.Shared);

            return result;
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
            return isConnectedToServer;
        }

        public bool JoinRoom(string roomName)
        {
            var result = Runner.StartGame(new StartGameArgs()
            {
                SessionName = roomName
            }
            );

            return true;
        }

        public GameObject LinkNetworkObject(GameObject remotePrefab, GameObject localPrefab)
        {
            if (remotePrefab.name != localPrefab.name)
            {
                return null;
            }

            var remotePview = remotePrefab.GetComponent<NetworkObject>();
            var ID = remotePview.Id;
            var newNetObject = localPrefab.GetOrAddComponent<NetworkObject>();
            //PhotonNetwork.LocalCleanPhotonView(remotePview);

            remotePrefab.SetActive(false);
            Runner.Despawn(remotePview);
            Destroy(remotePrefab);
            //newNetObject = ID;
            localPrefab.GetOrAddComponent<SyncTransform>().Initialise();

            return localPrefab;
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            isConnectedToServer = true;
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.LogError("Connection to Remote Address: " + remoteAddress.ToString() + " failed\n"
                            + "Reason: " + reason.ToString()
                );
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("Request Received: " + request);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log("User " + runner.UserId + " has Disconnected\nReason: " + reason);
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            throw new NotImplementedException();
        }

        //no need to implement
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        //No need to implement
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            throw new NotImplementedException();
        }

        //No need to implement
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        //No need to implement
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
            throw new NotImplementedException();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer == player)
            {
                Debug.Log("Local Player Joined!!");
            }
            else
            {
                Debug.Log("Another player joined!!");
            }
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
            Debug.Log("Scene Loading has finished");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("Scene Loading has started");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            allRoomsInfo = sessionList;
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("Shutdown from user: " + runner.UserId + "\nReason " + shutdownReason);
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