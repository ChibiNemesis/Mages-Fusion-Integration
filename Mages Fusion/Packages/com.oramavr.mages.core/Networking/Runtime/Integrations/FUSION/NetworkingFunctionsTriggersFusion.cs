namespace MAGES.UIs
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using MAGES.Networking;
    using TMPro;
    using UnityEngine.UI;

    public class NetworkingFunctionsTriggersFusion : NetworkingFunctionTriggers
    {
        [SerializeField]
        Button JoinButton;
        [SerializeField]
        GameObject SessionList;
        [SerializeField]
        GameObject SessionButtonPrefab;

        private FUSIONIntegration Integration;
        private GameObject PreviousSelected;

        private void Start()
        {
            Integration = GameObject.Find("Hub").GetComponent<FUSIONIntegration>();
            AddSessions();
        }

        private void AddSessions()
        {
            var AllRoomsInfo = Integration.GetAvailableSessions();

            if (AllRoomsInfo == null)
                return;

            var count = 0;
            foreach(var session in AllRoomsInfo)
            {
                var copy = Instantiate(SessionButtonPrefab);
                copy.transform.position += new Vector3(copy.transform.position.x,
                    copy.transform.position.y - copy.GetComponent<RectTransform>().rect.height * count,
                    copy.transform.position.z);

                copy.GetComponent<SessionButtonHandler>().Trigger = this;
                copy.transform.Find("SessionName").GetComponent<TMP_Text>().text = session.Name;
                copy.transform.Find("SessionPlayers").GetComponent<TMP_Text>().text = session.PlayerCount.ToString();
                copy.transform.parent = SessionList.transform;
                count++;
            }
        }

        public void Back() {
            Instantiate(OperationStartUI);
            Destroy(this.gameObject);
        }

        public void CreateRoom() {
            Integration.CreateRoom("Fusion Session");
            Instantiate(CreateSessionUI);
            Destroy(this.gameObject);
        }

        public void JoinSession() {
            if (SelectedSession != null)
            {
                var SessionName = SelectedSession.transform.Find("SessionName").GetComponent<TMP_Text>().text;
                Integration.JoinRoom(SessionName);
            }
        }

        public void SetSelectedSession(GameObject Session)
        {
            if (PreviousSelected != null)
            {
                PreviousSelected.transform.Find("Background").GetComponent<Image>().color = new Color(0,0,0,10);
            }
            SelectedSession = Session;
            PreviousSelected = Session;

            SelectedSession.transform.Find("Background").GetComponent<Image>().color = new Color(0, 0, 0, 150);

            JoinButton.interactable = true;
        }
    }
}