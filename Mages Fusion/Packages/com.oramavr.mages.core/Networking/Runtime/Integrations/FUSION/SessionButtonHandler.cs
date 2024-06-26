namespace MAGES.UIs
{

    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class SessionButtonHandler : MonoBehaviour
    {
        [SerializeField] public  NetworkingFunctionsTriggersFusion Trigger;

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetSession()
        {
            if(Trigger != null)
            {
                Trigger.SetSelectedSession(this.gameObject);
            }
        }
    }

}
