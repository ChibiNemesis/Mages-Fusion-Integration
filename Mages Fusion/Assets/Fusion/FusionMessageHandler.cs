using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MAGES;
using MAGES.Interaction.Interactables;
using MAGES.Interaction.Interactors;
using MAGES.SceneGraph;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
public class FusionMessageHandler : MonoBehaviour
{
    private NetworkObject NetObject;
    private NetworkRunner Runner;

    public void SetRunner(NetworkRunner runner)
    {
        Runner = runner;
    }

    public void RequestStateChange(byte code, string actionID)
    {
        
    }

    [Rpc]
    public void StateChangeFromClient(byte code, string actionID)
    {
        var baseActionData = Hub.Instance.Get<MAGESSceneGraph>().Runner.RuntimeGraph.Find(actionID);
        switch (code)
        {
            case 0:
                Hub.Instance.Get<MAGESSceneGraph>().Runner.PerformAction(baseActionData);
                break;
            case 1:
                Hub.Instance.Get<MAGESSceneGraph>().Runner.UndoAction(baseActionData);
                break;
        }
    }

    // RpcSources.All: anyone can call this RPC
    // RpcTargets.All: send this to everyone
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DestroyComponent(int viewID, string componentType)
    {
        NetworkObject obj;
        //Runner.Despawn();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
