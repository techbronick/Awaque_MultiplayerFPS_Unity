using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    private PhotonView PV;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        //Detaching the current camera from the Player Controller prefab
        transform.parent = null;
    }


    private void Start()
    {
        if (!PV.IsMine)
        {
            //If the photon view is not mine set other players cameras and audioListeners off on the current machine
            GetComponentsInChildren<Camera>()[0].enabled = false;
            GetComponentsInChildren<Camera>()[1].enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
        else
        {
            //Set the corresponding tags for Camera Holder And FPScam
            this.gameObject.tag = "LocalCameraHolder";
            GetComponentsInChildren<Camera>()[0].gameObject.tag = "LocalFpsCam";
        }


    }


    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
