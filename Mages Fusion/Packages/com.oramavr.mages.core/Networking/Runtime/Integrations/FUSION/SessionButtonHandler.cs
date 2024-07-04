namespace MAGES.UIs
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SessionButtonHandler : MonoBehaviour
    {
        public  NetworkingFunctionsTriggersFusion Trigger;

        public void SetSession()
        {
            if(Trigger != null)
            {
                this.gameObject.transform.Find("Background").GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
                Trigger.SetSelectedSession(gameObject);
            }
        }
    }

}
