using UnityEngine;
using Photon.Pun;

public class TakeDamage : MonoBehaviour
{
    private PhotonView PV;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    [PunRPC]
    public void RPC_TakeDamage(int myPhotonViewId, int damageTaken)
    {
        //Calls PlayerDamaged function for the damaged player through the network
        PhotonView.Find(myPhotonViewId).GetComponentInChildren<PlayerHealth>().PlayerDamaged(damageTaken);
    }
}
