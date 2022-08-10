using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip stepSFX;
    public AudioClip jumpSFX;
    public AudioClip landSFX;
    public AudioClip onWallSFX;
    public AudioClip onAirSFX;

    public AudioSource audioSource;

    public PlayerMovementNew playerMovement;
    public WallRun wallRun;

    public float stepDelayOnGround;
    public float stepDelayOnWall;

    private PhotonView PV;

    private float nextFootstep = 0;

    public void Awake()
    {
         PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W)) && playerMovement.isGrounded)
        {
            nextFootstep -= Time.deltaTime;
            if (nextFootstep <= 0)
            {
                PV.RPC("RPC_PlaySound",RpcTarget.All, PV.ViewID, 1);
                nextFootstep += stepDelayOnGround;
            }
        }

        if (Input.GetKey(KeyCode.W) && wallRun.onWall)
        {
            nextFootstep -= Time.deltaTime;
            if (nextFootstep <= 0)
            {
                PV.RPC("RPC_PlaySound", RpcTarget.All, PV.ViewID, 2);
                nextFootstep += stepDelayOnWall;
            }
        }

        if ((wallRun.onWall || playerMovement.isGrounded) && Input.GetKeyDown(KeyCode.Space))
        {
            PV.RPC("RPC_PlaySound", RpcTarget.All, PV.ViewID, 3);
        }

        if (playerMovement.isGrounded && !playerMovement.wasGrounded)
        {
            PV.RPC("RPC_PlaySound", RpcTarget.All, PV.ViewID, 4);
        }
    }
    public void PlaySound(int soundID)
    {
        switch(soundID)
        {
            case 1:
                audioSource.PlayOneShot(stepSFX);
                break;
            case 2:
                audioSource.PlayOneShot(onWallSFX);
                break;
            case 3:
                audioSource.PlayOneShot(jumpSFX);
                break;
            case 4:
                audioSource.PlayOneShot(landSFX);
                break;
        }
            
    }

    [PunRPC]
    void RPC_PlaySound(int senderID,int soundID)
    {
        PhotonView.Find(senderID).GetComponent<PlayerSounds>().PlaySound(soundID);
    }
}
