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
    using MAGES.UIs;

    public class FUSIONIntegration : NetworkBehaviour, IMAGESNetworkIntegration, INetworkRunnerCallbacks
    {
        [SerializeField]
        GameObject CharacterAvatar;

        [Networked,Capacity(100)]
        private NetworkDictionary<PlayerRef, NetworkObject> players { get; set; }

        private NetworkRunner _runner;
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

        public List<SessionInfo> GetAvailableSessions()
        {
            return allRoomsInfo;
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
        }

        public bool CreateRoom(string roomName)
        {
            _ = CreateRoomInner(roomName);

            return true;
        }

        private async Task<StartGameResult> CreateRoomInner(string roomName)
        {
            var result = await _runner.StartGame(new StartGameArgs()
            {
                SessionName = roomName,
                PlayerCount = 20,
                IsOpen = true,
                IsVisible = true,
                GameMode = GameMode.Shared
            });

            return result;
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
                _runner.Despawn(NetObject);
            }
            else if (NetObject != null)
            {
                //This may not be needed unless the object does not have state authority
                Debug.Log("add callback or do something similar with transfer ownership...");
            }
            else
            {
                Debug.LogError("The object you are trying to network-destroy is not a network object.");
                return false;
            }

            return true;
        }

        public void Disconnect()
        {
            isConnectedToServer = false;
            returnCode = null;
            _runner.Disconnect(_runner.LocalPlayer);
        }

        public bool EstablishConnectionToMainServer(string args)
        {
            var task = EstablishConnectionToMainServerInner();

            //return task.Result.Ok;
            return true;
        }

        private async Task<StartGameResult> EstablishConnectionToMainServerInner()
        {
            var result = await _runner.JoinSessionLobby(SessionLobby.Shared);
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
            if (_runner.SessionInfo != null)
            {
                return _runner.SessionInfo.PlayerCount;
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
            if (_runner.SessionInfo != null)
            {
                return _runner.SessionInfo.Name;
            }

            return null;
        }

        public int GetNetworkID(GameObject networkObject)
        {
            if (networkObject == null)
            {
                return -1;
            }

            var NetObject = networkObject.GetComponent<NetworkObject>();
            if (NetObject == null)
            {
                return -1;
            }

            return (int)NetObject.Id.Raw;
        }

        public int GetPing()
        {
            throw new NotImplementedException();
        }

        public int GetPrefabIDFromNetwork(GameObject prefab, out bool isUnique)
        {
            throw new NotImplementedException();
        }

        public bool HasAuthority(GameObject networkObject)
        {
            var NetObject = networkObject.GetComponent<NetworkObject>();
            {
                Debug.LogError("Has authority called for object " + gameObject.name + " which is not a network object.");
            }

            return NetObject.HasStateAuthority;
        }

        public void InitSpawnedObjectForNetwork(GameObject gameObject, bool syncTransform)
        {}

        public bool IsConnectedToServer()
        {
            return isConnectedToServer;
        }

        public bool JoinRoom(string roomName)
        {
            var result = _runner.StartGame(new StartGameArgs()
            {GameMode = GameMode.Shared, SessionName = roomName});

            return true;
        }

        public GameObject LinkNetworkObject(GameObject remotePrefab, GameObject localPrefab)
        {
            if (remotePrefab.name != localPrefab.name)
            {
                return null;
            }

            var NetObject = remotePrefab.GetComponent<NetworkObject>();
            var ID = NetObject.Id;
            var newNetObject = localPrefab.GetOrAddComponent<NetworkObject>();
            //PhotonNetwork.LocalCleanPhotonView(remotePview);

            remotePrefab.SetActive(false);
            //PhotonNetwork.PrefabPool.Destroy(remotePrefab); -> use an INetworkObjectProvider
            _runner.Despawn(NetObject);
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
        {}

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {}

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
            Debug.Log("User " + runner.UserId + " has Disconnected\nReason: " + reason);
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {}

        //no need to implement
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {}

        //No need to implement
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {}

        //No need to implement
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {}

        //No need to implement
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {}

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            NetworkObject Avatar;
            GameObject rig;
            GameObject Camera;
            if (runner.LocalPlayer == player)
            {
                Debug.Log("Local Player Joined!!");
                if (Hub.Instance.RuntimeBundle.DeviceManager.CurrentMode == DeviceManagerModule.CameraMode.Mobile3D)
                {
                    rig = GameObject.Find("MobileRig");
                    Camera = rig.transform.Find("Camera").gameObject;
                    Avatar = runner.Spawn(CharacterAvatar, rig.transform.position,Camera.transform.rotation);
                    Avatar.transform.Find("AvatarOffset").gameObject.SetActive(false);
                    Avatar.gameObject.name = "Local Avatar";
                    runner.SetPlayerObject(player, Avatar);
                }
            }
            /*else
            {
                Debug.Log("Another player joined!!");
                if (Hub.Instance.RuntimeBundle.DeviceManager.CurrentMode == DeviceManagerModule.CameraMode.Mobile3D)
                {
                    rig = GameObject.Find("MobileRig");
                    Camera = rig.transform.Find("Camera").gameObject;
                    Avatar = runner.Spawn(CharacterAvatar, rig.transform.position, Camera.transform.rotation);
                    runner.SetPlayerObject(player, Avatar);
                }
            }*/
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player: " + player.PlayerId + " has left");

            NetworkObject NetObject = runner.GetPlayerObject(player);

            if(NetObject != null)
            {
                runner.Despawn(NetObject);
            }
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {}

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {}

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

            GameObject.Find("NetworkingUIMedical Fusion(Clone)").GetComponent<NetworkingFunctionsTriggersFusion>().AddSessions();
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("Shutdown from user: " + runner.UserId + "\nReason " + shutdownReason);
        }

        public void OnStartup()
        {
            Hub.Instance.Get<NetworkingModule>().NetworkIdType = typeof(NetworkObject); //Fusion
            networking = Hub.Instance.Get<MAGESNetworking>();

            if (!gameObject.GetComponent<NetworkObject>())
            {
                gameObject.AddComponent<NetworkObject>();
            }

            _runner = GameObject.Find("Runner").GetComponent<NetworkRunner>();
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log(message.ToString());
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
                Debug.LogError($"Network Request Ownership called for object {gameObject.name}, which does not have a NetworkObject component.");
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

            GameObject spawnedObject = null;

            if (prefab.GetComponent<NetworkObject>())
            {
                spawnedObject =  _runner.Spawn(prefab, 
                    prefab.transform.position, 
                    prefab.transform.rotation).gameObject;
            }
            else
            {
                spawnedObject = Instantiate(prefab);

                var NetObject = spawnedObject.AddComponent<NetworkObject>();
                NetObject.ReleaseStateAuthority(); // give authority to other players
            }

            //if (spawnedObject.GetComponent<Rigidbody>())
            //{
            //    spawnedObject.GetOrAddComponent<SyncTransformFusion>();
            //}

            return spawnedObject;
        }

        public short? ValidateClientNetworkingSetup()
        {
            return returnCode;
        }
    }
}