namespace MAGES.UIs
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using MAGES.Networking;

    public class NetworkingFunctionsTriggersFusion : NetworkingFunctionTriggers
    {
        private FUSIONIntegration Integration;

        private void Start()
        {
            Integration = GameObject.Find("Hub").GetComponent<FUSIONIntegration>();
        }

        public void Back() {
            Instantiate(OperationStartUI);
            Destroy(this.gameObject);
        }

        public void CreateRoom() {
            Integration.CreateRoom("Fusion Session");
        }

        public void JoinSession() {
            Integration.JoinRoom("Fusion Session");
        }
    }
}