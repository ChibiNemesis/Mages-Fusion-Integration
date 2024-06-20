namespace MAGES.UIs
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using MAGES.Networking;

    public class OperationExitTriggersFusion : OperationExitTriggers
    {
        private FUSIONIntegration Integration;

        private void Start()
        {
            Integration = GameObject.Find("Hub").GetComponent<FUSIONIntegration>();
        }
        public void Exit() {
            Integration.Disconnect();
            Application.Quit();
        }

        public void ReloadScene() { 
            //TODO
        }
    }
}