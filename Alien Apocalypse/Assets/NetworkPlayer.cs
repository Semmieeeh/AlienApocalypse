using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
