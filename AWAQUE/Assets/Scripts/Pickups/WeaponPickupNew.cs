using UnityEngine;
using Photon.Pun;

public class WeaponPickupNew : MonoBehaviourPun
{
    public AudioClip PickupSfx;

    public GameObject PickupVfxPrefab;

    public Rigidbody PickupRigidbody {get; private set;}

    public WeaponScript weaponScript;

    public WeaponBobbing weaponBobbing;

    public Transform weaponHolder;

    public Collider m_Collider;

    public bool picked;
    public bool HasPlayedFeedback;

    private PhotonView PV;


    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

  
    void Start()
    {
        picked = false;
        PickupRigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        if (m_Collider == null)
        {
            m_Collider = this.gameObject.GetComponentInChildren<BoxCollider>();
        }

        PickupRigidbody.isKinematic = true;
        m_Collider.isTrigger = true;
    }


    void Update()
    {
        
        //If the current weapon has lost it's parent(after player's death) set it current's layer to 0
        if (PV.IsMine && picked && transform.parent == null)
        {
            foreach (var kid in this.transform.GetComponentsInChildren(typeof(Transform), true))
            {
                kid.gameObject.layer = 0;
            }

            //Call the RPC function RPC_OnWeaponDetached
            PV.RPC("RPC_OnWeaponDetached", RpcTarget.All, PV.ViewID);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        //Get the photon view from the picker
        PhotonView otherPV = other.GetComponent<PhotonView>();

        //If picker's photon view exists and it is his photonview
        if (otherPV != null && otherPV.IsMine)
        {
            //Call the RPC function RPC_OnTriggerEnter
            PV.RPC("RPC_OnTriggerEnter", RpcTarget.All, otherPV.ViewID);

            //Set the weapon layer to 8, this is used with the 2nd camera attached on the player which prevents weapon clipping 
            foreach (var kid in this.transform.GetComponentsInChildren(typeof(Transform), true))
            {
                kid.gameObject.layer = 8;
            }
            weaponBobbing.enabled = true;
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
                Instantiate(PickupVfxPrefab, transform.position, Quaternion.identity);
            }

            HasPlayedFeedback = true;
    }


    [PunRPC]
    void RPC_OnTriggerEnter(int senderId)
    {
        //RPC function which will send a message to all the players that the current weapon was atached and it will set the according variables through the network for the current weapon
        if (picked) return;
        PlayPickupFeedback();

        //Find the player who picked this item
        GameObject player = PhotonView.Find(senderId).gameObject;

        //Get the weaponHolder from reference on player's Camera look component
        weaponHolder = player.GetComponent<CameraLook>().cam.GetChild(0);

        //Verify if the player is not carrying this type of weapon, if it does, destroy this object and fulfill player's ammo on this weapon 
        foreach (var child in weaponHolder.GetComponentsInChildren(typeof(Transform), true))
        {
            if (child.gameObject.tag == this.gameObject.tag)
            {
                Destroy(this.gameObject);
                child.GetComponent<WeaponScript>().ammoIn();
                return;
            }
        }

        //Transform the ownership of this weapon to the picking player(To be able to controll it)
        PV.TransferOwnership(player.GetComponent<PhotonView>().Owner);

        //Set corresponding variables and transform positions 
        picked = true;
        m_Collider.enabled = false;
        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
        weaponScript.enabled = true;
        weaponScript.fpsCam = weaponHolder.GetComponentInParent<Camera>();
        weaponScript.playerRb = player.GetComponent<Rigidbody>();
        weaponHolder.GetComponent<WeaponManager>().WeaponAttached();

    }


    [PunRPC]
    void RPC_OnWeaponDetached(int weaponId)
    {
        //RPC function which will send a message to all the players that the current weapon was detached and it will set the according variables through the network for the current weapon
        WeaponPickupNew weapon = PhotonView.Find(weaponId).GetComponent<WeaponPickupNew>();
        weapon.m_Collider.enabled = true;
        weapon.weaponScript.enabled = false;
        weapon.weaponBobbing.enabled = false;
        weapon.picked = false;
        weapon.HasPlayedFeedback = false;
        weapon.weaponHolder.GetComponent<WeaponManager>().weaponsPicked--;
        weapon.weaponHolder = null;
    }


}
