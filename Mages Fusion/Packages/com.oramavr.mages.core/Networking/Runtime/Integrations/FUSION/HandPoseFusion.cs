#if FUSION2

namespace MAGES.Networking
{
    using MAGES.Interaction.Interactors;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class HandPoseFusion : SyncTransformFusion
    {
        private void Start()
        {
            if (!NetObject.HasStateAuthority)
            {
                Initialise(null);
            }
        }

        public void Initialise(HandInteractor handInteractor)
        {
            if (NetObject.HasStateAuthority)
            {
                var handVisual = handInteractor.transform.Find("HandVisual").Find("Root");
                InitTransforms(handVisual.gameObject);
            }
            else
            {
                InitTransforms(transform.Find("HandVisual").Find("Root").gameObject);
            }
        }

        private void InitTransforms(GameObject handVisual)
        {
        }
    }

}

#endif