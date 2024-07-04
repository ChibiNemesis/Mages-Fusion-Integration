namespace MAGES.UIs
{

    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class CreateSessionTriggersFusion : CreateSessionTriggers
    {
        [SerializeField]
        public TMP_Text SessionName;
        public void StartSession()
        {
            base.StartSession();
        }
    }
}