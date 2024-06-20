namespace MAGES.UIs
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using MAGES.Networking;

    public class OperationStartTriggersFusion : OperationStartTriggers
    {
        private FUSIONIntegration Integration;

        public GameObject SinglePlayerButton;
        public GameObject MultiPlayerButton;
        public GameObject CustomizationButton;
        public GameObject QuitButton;
        public GameObject LoadingUI;

        private void Start()
        {
            Integration = GameObject.Find("Hub").GetComponent<FUSIONIntegration>();
        }

        public void AvatarConfiguration() {
            Instantiate(AvatarCustomizationUI);
            Destroy(this.gameObject);
        }

        public void Exit() {
            Application.Quit();
        }

        public void MultiPlayer() {
            LoadingUI.SetActive(true);
            SinglePlayerButton.SetActive(false);
            MultiPlayerButton.SetActive(false);
            CustomizationButton.SetActive(false);
            QuitButton.SetActive(false);
            
            var result = Integration.EstablishConnectionToMainServer("");
            if (result)
            {
                Instantiate(NetworkingUI);
                Destroy(this.gameObject);
            }
        }

        public void SinglePlayer() {
            
            base.SinglePlayer();
        }
    }

}
