using MAGES;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncLocalAvatarTransform : MonoBehaviour
{
    public GameObject rig;
    public GameObject Camera;
    void Start()
    {
        if(Hub.Instance.RuntimeBundle.DeviceManager.CurrentMode == DeviceManagerModule.CameraMode.Mobile3D)
        {
            rig = GameObject.Find("MobileRig");
            Camera = rig.transform.Find("Camera").gameObject;
        }
    }
    void FixedUpdate()
    {
        gameObject.transform.position = rig.transform.position;
        gameObject.transform.rotation = Camera.transform.rotation;
    }
}
