using UnityEngine;
using Photon.Pun;


public class HealthPickup : MonoBehaviour
{

    public AudioClip PickupSfx;

    public GameObject PickupVfxPrefab;

    public Rigidbody PickupRigidbody { get; private set; }
    
    [HideInInspector]
    public BoxCollider m_Collider;

    public bool picked;

    private bool HasPlayedFeedback;

    private PhotonView PV;

    private Animator animator;


    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    private void Start()
    {
        PickupRigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        if (m_Collider == null)
        {
            m_Collider = this.gameObject.GetComponentInChildren<BoxCollider>();
        }

        picked = false;
        PickupRigidbody.isKinematic = true;
        m_Collider.isTrigger = true;
    }

    private void Update()
    {
        if (!picked)
            animator.Play("Base Layer.BobbingAndRotatingAnim", 0);
    }

    void OnTriggerEnter(Collider other)
    {
        //Get the photon view from the picker
        PhotonView otherPV = other.GetComponent<PhotonView>();

        //If picker's photon view exists and it is his photonview
        if (otherPV != null && otherPV.IsMine)
        {
            //If the picker's current health is 100 do nothing
            if (other.GetComponent<PlayerHealth>().currentHealth == 100) return;

            //Call the RPC_OnTriggerEnter RPC function
            PV.RPC("RPC_OnTriggerEnter", RpcTarget.All, otherPV.ViewID);
            return;
        }
    }


    void PlayPickupFeedback()
    {
            //Manages the visual and audio feedback after successful pickup
            if (HasPlayedFeedback)
                return;

            if (PickupSfx)
            {
                AudioSource.PlayClipAtPoint(PickupSfx, this.transform.position);
            }

            if (PickupVfxPrefab)
            {
                var pickupVfxInstance = Instantiate(PickupVfxPrefab, transform.position, Quaternion.identity);
            }
            HasPlayedFeedback = true;
        
    }


    [PunRPC]
    void RPC_OnTriggerEnter(int senderId)
    {
        //Rpc function which let know other users through newtwork that a concrete player picked the health pickupble
        if (picked) return;
        PlayPickupFeedback();

        //Find the player who picked this item
        GameObject player = PhotonView.Find(senderId).gameObject;

        //Call the HealthIn function for this player 
        player.GetComponent<PlayerHealth>().HealthIn();
        Destroy(this.gameObject);
        picked = true;
    }
}
