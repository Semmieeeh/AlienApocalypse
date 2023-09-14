using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{
    protected Movement m;
    protected Vector3 remotePlayerPosition;

    

    private void Awake()
    {
        m = GetComponent<Movement>();
        if(!photonView.IsMine && GetComponent<Movement>()!= null)
        {
            Destroy(GetComponent<Movement>());
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.IsMine)
        {
            return;
        }

        
        if (!photonView.IsMine && GetComponent<Movement>() != null)
        {
            Destroy(GetComponent<Movement>());
        }

        if (photonView.IsMine)
        {
            return;
        }
        var lagDistance = remotePlayerPosition - transform.position;

        if(lagDistance.magnitude > 0.5f)
        {
            transform.position = remotePlayerPosition;
            lagDistance = Vector3.zero;
        }

        if(lagDistance.magnitude < 0.1)
        {
            m.input.x = 0;
            m.input.y = 0;

        }
        else
        {
            m.input.x = lagDistance.normalized.x;
            m.input.y = lagDistance.normalized.y;
        }

        
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);

        }
        else
        {
            remotePlayerPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
