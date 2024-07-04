namespace MAGES.UIs
{
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
        [SerializeField]
        GameObject CharacterAvatar;

        private FUSIONIntegration Integration;
        private GameObject PreviousSelected;

        private void Start()
        {
            Integration = GameObject.Find("Hub").GetComponent<FUSIONIntegration>();
        }

        public void AddSessions()
        {
            //Delete Previous list from ui
            var PreviousList = SessionList.transform;
            for(var i=0;i< PreviousList.childCount; i++)
            {
                Destroy(PreviousList.GetChild(i).gameObject);
            }

            var AvailableRooms = Integration.GetAvailableSessions();

            if (AvailableRooms == null)
                return;

            var count = 0;
            foreach(var session in AvailableRooms)
            {
                Debug.Log("Adding Session to List");
                AddSessionButton(session.Name, session.PlayerCount, count);
                count++;
            }
        }

        private void AddSessionButton(string SessionName, int JoinedPlayers, int count)
        {
            var copy = Instantiate(SessionButtonPrefab);
            copy.transform.position = new Vector3(copy.transform.position.x,
                copy.transform.position.y - copy.GetComponent<RectTransform>().rect.height * count,
                copy.transform.position.z);

            copy.GetComponent<SessionButtonHandler>().Trigger = this;
            copy.transform.Find("SessionName").GetComponent<TMP_Text>().text = SessionName;
            copy.transform.Find("SessionPlayers").GetComponent<TMP_Text>().text = JoinedPlayers.ToString();
            copy.transform.SetParent(SessionList.transform, false);
        }

        public void Back() {
            Instantiate(OperationStartUI);
            Destroy(this.gameObject);
        }

        public void CreateRoom() {

            var rnd = new System.Random();

            string roomName = "Session-" + rnd.Next(10) + rnd.Next(10) + rnd.Next(10) + rnd.Next(10);

            Integration.CreateRoom(roomName);
            var session = Instantiate(CreateSessionUI);
            session.GetComponent<CreateSessionTriggersFusion>().SessionName.text = roomName;

            Destroy(gameObject);
        }

        public void JoinSession() {
            if (PreviousSelected != null)
            {
                var SessionName = PreviousSelected.transform.Find("SessionName").GetComponent<TMP_Text>().text;
                Integration.JoinRoom(SessionName);

                var session = Instantiate(CreateSessionUI);
                session.GetComponent<CreateSessionTriggersFusion>().SessionName.text = SessionName;

                Destroy(gameObject);
            }
        }

        public void SetSelectedSession(GameObject Session)
        {

            if (PreviousSelected != null)
            {
                PreviousSelected.transform.Find("Background").GetComponent<Image>().color = new Color(0,0,0,0);
            }
            PreviousSelected = Session;

            JoinButton.interactable = true;
        }
    }
}