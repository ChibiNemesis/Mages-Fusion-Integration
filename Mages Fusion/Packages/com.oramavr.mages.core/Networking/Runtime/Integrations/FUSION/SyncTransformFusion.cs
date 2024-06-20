#if FUSION2

namespace MAGES.Networking
{
    using Fusion;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SyncTransformFusion : NetworkBehaviour
    {
        protected NetworkObject NetObject { get; private set; }

        [Networked]
        private Vector3 Pos { get; set; }

        [Networked]
        private Quaternion Rot { get; set; }

        public override void Spawned()
        {
            base.Spawned();
            if (gameObject.GetComponent<NetworkObject>())
            {
                NetObject = gameObject.GetComponent<NetworkObject>();
            }
            else
            {
                NetObject = gameObject.AddComponent<NetworkObject>();
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (NetObject.HasStateAuthority)
            {
                Pos = gameObject.transform.position;
                Rot = gameObject.transform.rotation;
            }
            else
            {
                gameObject.transform.position = Pos;
                gameObject.transform.rotation = Rot;
            }
        }
    }
}
#endif