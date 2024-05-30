using Fusion;
using MAGES.Networking;
using MAGES.RigidBodyAnimation;
using MAGES.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MAGESObject))]
public class SyncTransformFusion : SyncTransform
{
    protected NetworkObject FusionViewRef { get; private set; }

    public override void Initialise()
    {
        base.Initialise();
        FusionViewRef = this.GetComponent<NetworkObject>(); //probably correct
        if (FusionViewRef == null)
        {
            return;
        }
    }

    private struct TransformSendData
    {
        public TransformFlags Flags;

        public int ID;
        public Vector3 Pos;

        public Quaternion Rot;
    }
}
