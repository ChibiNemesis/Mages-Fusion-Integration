namespace MAGES.Networking
{
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
        private NetworkRunner Runner;

        public void RequestStateChange(byte code, string actionID)
        {
            StateChangeFromClient(code, actionID);
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

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void DestroyComponent(NetworkObject _NetObj, string componentType)
        {
            var NetObjectComponent = _NetObj.gameObject.GetComponent(componentType);
            Destroy(NetObjectComponent);
        }

        public void ActivatedGrabbable(ActivateEnterInteractionEventArgs eventArgs)
        {
            var grabbableObject = (Grabbable)eventArgs.Interactable;
            if (grabbableObject != null)
            {
                var NetObject = grabbableObject.GetComponent<NetworkObject>();
                RegisterActivation(NetObject.Id, true);
            }
        }

        public void DeActivateGrabbable(ActivateExitInteractionEventArgs eventArgs)
        {
            var grabbableObject = (Grabbable)eventArgs.Interactable;
            if (grabbableObject != null)
            {
                var NetObject = grabbableObject.GetComponent<NetworkObject>();
                RegisterActivation(NetObject.Id, false);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RegisterActivation(NetworkId ID, bool activated)
        {
            NetworkObject NetObject;

            if (!Runner.TryFindObject(ID, out NetObject))
            {
                Debug.LogError("Could not register activation for viewID " + ID + ". ViewID not found.");
            }
            else
            {
                var activateObject = NetObject.gameObject;
                Hub.Instance.Get<NetworkingModule>().RegisterActivatedObject(activateObject, activated);
            }
        }

        public void AddActivationListeners(HandInteractor handInteractor)
        {
            handInteractor.ActivateEntered.AddListener(ActivatedGrabbable);
            handInteractor.ActivateExited.AddListener(DeActivateGrabbable);
        }

        void Start()
        {
            Runner = GameObject.Find("Runner").GetComponent<NetworkRunner>();
            var integration = (FUSIONIntegration)Hub.Instance.Get<NetworkingModule>().Integration;
            integration.NetworkMessageHandler = this;
            var interactionSystem = Hub.Instance.Get<InteractionSystemModule>();
            AddActivationListeners(interactionSystem.LeftHand.GetComponent<HandInteractor>());
            AddActivationListeners(interactionSystem.RightHand.GetComponent<HandInteractor>());
        }
    }
}