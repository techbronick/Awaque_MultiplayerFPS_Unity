using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponManager : MonoBehaviourPun
{
    public int previousdWeapon;
    public int selectedWeapon;
    public int weaponsPicked;

    public PlayerHealth playerHealth;
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        selectedWeapon = 0;
        weaponsPicked = 0;
    }
    void Update()
    {

        if (PV.IsMine)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1) && weaponsPicked > 1)
            {
                previousdWeapon = selectedWeapon;
                selectedWeapon = 0;
                PV.RPC("RPC_SelectWeapon", RpcTarget.All, PV.ViewID, selectedWeapon);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2 && weaponsPicked >= 2)
            {
                previousdWeapon = selectedWeapon;
                selectedWeapon = 1;
                PV.RPC("RPC_SelectWeapon", RpcTarget.All, PV.ViewID, selectedWeapon);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3 && weaponsPicked == 3)
            {
                previousdWeapon = selectedWeapon;
                selectedWeapon = 2;
                PV.RPC("RPC_SelectWeapon", RpcTarget.All, PV.ViewID, selectedWeapon);
            }

            if (Input.GetKeyDown(KeyCode.Q) && weaponsPicked > 1)
            {
                (selectedWeapon, previousdWeapon) = (previousdWeapon, selectedWeapon);
                PV.RPC("RPC_SelectWeapon", RpcTarget.All, PV.ViewID, selectedWeapon);

            }

            if (playerHealth.isDead == true)
            {
                PV.RPC("RPC_DetachWeapons", RpcTarget.All, PV.ViewID, transform.position);
            }
        }
    }

    public void WeaponAttached()
    {
        //Function which incremnts the number of picked weapons, selects the picked weapon
        if (PV.IsMine)
        {
            weaponsPicked++;
            previousdWeapon = selectedWeapon;
            selectedWeapon = weaponsPicked - 1;

            //Call for RPC_SelectWeapon RPC function
            PV.RPC("RPC_SelectWeapon", RpcTarget.All, PV.ViewID, selectedWeapon);
        }
    }



    [PunRPC]
    public void RPC_SelectWeapon(int senderWeaponManagerId, int selectedWeapon)
    {
        //Rpc function which changes the current player's weapon through the network 
        Transform weaponManager = PhotonView.Find(senderWeaponManagerId).transform;
        int i = 0;
        foreach (Transform weapon in weaponManager)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }


    [PunRPC]
    void RPC_DetachWeapons(int weaponHolderId, Vector3 weaponHolderPosition)
    {
        //Rpc function used to detach all the weapons transform from the dead player and set them Active 
        Transform weaponHolder = PhotonView.Find(weaponHolderId).transform;
        foreach (Transform child in weaponHolder)
        {
            child.position = weaponHolderPosition + new Vector3(Random.Range(0, 2.5f), 0, Random.Range(0, 2.5f));
            child.gameObject.SetActive(true);
        }
        weaponHolder.transform.DetachChildren();
    }
}
