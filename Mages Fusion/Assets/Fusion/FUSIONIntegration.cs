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

public class FUSIONIntegration : NetworkBehaviour, IMAGESNetworkIntegration
{
    private bool isConnectedToServer;
    //private List<RoomInfo> allRoomsInfo;
    private MAGESNetworking networking;
    //private PunMessageHandler networkMessageHandler;
    private short? returnCode = null;

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
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    public bool EstablishConnectionToMainServer(string args)
    {
        throw new System.NotImplementedException();
    }

    public List<string> GetAvailableRooms()
    {
        throw new System.NotImplementedException();
    }

    public int GetConnectedUsersToCurrentRoom()
    {
        throw new System.NotImplementedException();
    }

    public int GetConnectedUsersToRoom(string roomID)
    {
        throw new System.NotImplementedException();
    }

    public string GetCurrentConnectedRoom()
    {
        throw new System.NotImplementedException();
    }

    public int GetNetworkID(GameObject networkObject)
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    public GameObject LinkNetworkObject(GameObject remotePrefab, GameObject localPrefab)
    {
        throw new System.NotImplementedException();
    }

    public void OnStartup()
    {
        throw new System.NotImplementedException();
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
