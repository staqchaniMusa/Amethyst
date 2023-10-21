using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPunClientOwnershipChange
{
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer);
    

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner);

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest);
    
}
