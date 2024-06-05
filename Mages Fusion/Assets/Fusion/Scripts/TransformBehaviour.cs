#if FUSION2

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TransformBehaviour : NetworkBehaviour
{
    [Networked]
    Vector3 position { get; set; }
    [Networked]
    Quaternion orientation { get; set; }
    public override void FixedUpdateNetwork()
    {
        //Player object with state authority can only change Networked values
        if (this.HasStateAuthority)
        {
            if (position != this.gameObject.transform.position)
                position = this.gameObject.transform.position;

            if (orientation != this.gameObject.transform.rotation)
                orientation = this.gameObject.transform.rotation;
        }
        else
        {
            this.gameObject.transform.position = position;
            this.gameObject.transform.rotation = orientation;
        }
    }
}

#endif