#if FUSION2

namespace MAGES.Networking
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Fusion;
    using System;
    using MAGES.Interaction;

    public class MultiplayerAvatarFusion : MonoBehaviour
    {
        private NetworkObject NetObject;
        private NetworkRunner NetRunner;
        // Start is called before the first frame update
        void Start()
        {
            NetObject = GetComponent<NetworkObject>();
            NetRunner = GameObject.Find("Prototype Runner").GetComponent<NetworkRunner>();
            if (!NetObject)
            {
                Debug.LogError("MultiplayerAvatar requires a Network Object script.");
                return;
            }
            if (!NetRunner)
            {
                Debug.LogError("Could not Find Network Runner");
            }

            byte gender = 0;
            if (NetObject.HasStateAuthority)
            {
                var avatar = new AvatarData();
                avatar.Load();
                gender = Convert.ToByte(avatar.BodyType);
                //var localPlayerCustomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
                //localPlayerCustomProperties["Gender"] = gender;
                //PhotonNetwork.LocalPlayer.SetCustomProperties(localPlayerCustomProperties);
                ApplyGender(gender);
            }
        }

        private void ApplyGender(byte remoteGender)
        {
            var avatarOffset = transform.GetChild(0);
            for (var i = 0; i < avatarOffset.childCount; i++)
            {
                if (i != remoteGender)
                {
                    Destroy(avatarOffset.GetChild(i).gameObject);
                }
            }
        }
    }
}
#endif