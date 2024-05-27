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

public class FUSIONIntegration : NetworkBehaviour, IMAGESNetworkIntegration
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
    }

    public IMAGESObjectSynchronization AddSyncTransform(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }

    public bool CreateRoom(string roomName)
    {
        Runner.StartGame(new StartGameArgs()
        {
            SessionName = roomName,
            PlayerCount = 20,
            IsOpen = true,
            IsVisible = true,
        }
        );

        return true;
    }

    public bool DestroyComponent(GameObject gameObject, string componentType)
    {
        throw new System.NotImplementedException();

    }

    public bool DestroyObject(GameObject gameObject)
    {
        throw new System.NotImplementedException();
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
        if(Runner.SessionInfo != null)
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
        if(Runner.SessionInfo != null)
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

        return newtrowkView.HasStateAuthority;
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
        throw new System.NotImplementedException();
    }

    public GameObject LinkNetworkObject(GameObject remotePrefab, GameObject localPrefab)
    {
        throw new System.NotImplementedException();
    }

    public void OnStartup()
    {
        Hub.Instance.Get<NetworkingModule>().NetworkIdType = typeof(NetworkObject); //Fusion
        networking = Hub.Instance.Get<MAGESNetworking>();
    }

    public void RequestAuthority(GameObject networkObject)
    {
        throw new System.NotImplementedException();
    }

    public void RequestChangeState(byte changeState, string actionID)
    {
        throw new System.NotImplementedException();
    }

    public GameObject SpawnObject(GameObject prefab, bool isUnique = true)
    {
        throw new System.NotImplementedException();
    }

    public short? ValidateClientNetworkingSetup()
    {
        throw new System.NotImplementedException();
    }
}
