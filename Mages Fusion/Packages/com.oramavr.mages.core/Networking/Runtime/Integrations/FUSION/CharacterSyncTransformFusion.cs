namespace MAGES.Networking
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Fusion;

    public class CharacterSyncTransformFusion : NetworkBehaviour
    {
        [Networked]
        private Vector3 pos { set; get; }
        [Networked]
        private Quaternion rot { set; get; }

        private GameObject rig;
        private GameObject Camera;

        public override void Spawned()
        {
            if(Hub.Instance.RuntimeBundle.DeviceManager.CurrentMode == DeviceManagerModule.CameraMode.Mobile3D)
            {
                rig = GameObject.Find("MobileRig");
                Camera = rig.transform.Find("Camera").gameObject;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                pos = rig.transform.position;
                rot = Camera.transform.rotation;
            }
            else
            {
                gameObject.transform.position = pos;
                gameObject.transform.rotation = rot;
            }
        }
    }

}