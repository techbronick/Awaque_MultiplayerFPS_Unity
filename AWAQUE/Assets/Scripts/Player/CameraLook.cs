using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraLook : MonoBehaviour
{
    public Transform cam;

    [SerializeField] private WallRun wallRun;

    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform capsule;

    private PhotonView PV;

    private float mouseX;
    private float mouseY;

    private float multiplier = 0.01f;

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        MyInput();

        //Setting the camera and player's rotation
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        capsule.transform.rotation = orientation.transform.rotation;


    }

    private void MyInput()
    {
        //Getting the respective input axies
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        //Calculating the X and Y Rotation 
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        //Clamp is used do our player cannot look to far up or down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
