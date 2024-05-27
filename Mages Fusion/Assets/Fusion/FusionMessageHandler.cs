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

    [Rpc]
    public void DestroyComponent(int viewID, string componentType)
    {
        
    }

    public bool DestroyComponent(NetworkObject objectPhotonView, string componentType)
    {
        if (objectPhotonView != null)
        {
            photonView.RPC("DestroyComponent", RpcTarget.All, objectPhotonView.ViewID, componentType);
            return true;
        }

        return false;
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
