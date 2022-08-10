using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;

    public PlayerDeath playerDeath;
    public bool isDead { get; private set; }

    public Image damageVignette;

    public Slider slider;

    private Color alphaColor;

    private PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            alphaColor = damageVignette.color;
            alphaColor.a = 0;
            maxHealth = 100;
            currentHealth = maxHealth;
            SliderSetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (currentHealth > 0)
            isDead = false;

        SliderSetHealth(currentHealth);
        damageVignette.color = alphaColor;

        //Decrease the alphaColor on the damageVignette, after it's value change when the Player takes damage
        if (alphaColor.a > 0f)
            alphaColor.a -= 0.005f;
    }

    private void LateUpdate()
    {
        if (!PV.IsMine)
            return;

        if (!isDead)
            if (currentHealth <= 0)
            {
                Die();
                isDead = true;
            }
    }


    public void PlayerDamaged(float amount)
    {
        currentHealth -= amount;

        //If the player gets damaged set the alphaColor for the Damage Vignette
        alphaColor.a = 0.85f;
    }


    public void SliderSetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }


    public void SliderSetHealth(float health)
    {
        slider.value = health;
    }

    private void Die()
    {
        //Call the RPC_explode RPC function through the network
        PV.RPC("RPC_explode", RpcTarget.All, PV.ViewID);

        //Call ApplyDeathToOwner function
        ApplyDeathToOwner();

        //Checks if the player was not killed by his own bullet, if not calls the ApplyKillToOwner function with LastBulletPhotonViewID custom player propertie as parameter
        if (PV.ViewID != Convert.ToInt32(PV.Owner.CustomProperties["LastBulletPhotonViewID"]))
            ApplyKillToOwner(Convert.ToInt32(PV.Owner.CustomProperties["LastBulletPhotonViewID"]));
    }


    private void ApplyDeathToOwner()
    {
        if (PV.IsMine)
        {
            //Create a new hashtable which will store player's kills in player custom properties
            Hashtable hash = new Hashtable();
            int deathCount = Convert.ToInt32(PV.Owner.CustomProperties["Deaths"]);
            hash.Add("Deaths", deathCount + 1);
            PV.Owner.SetCustomProperties(hash);
        }
    }

    private void ApplyKillToOwner(int ownerPV)
    {
        if (PV.IsMine)
        {
            //Create a new hashtable which will store player's kills in player custom properties
            Hashtable hash = new Hashtable();            
            int killCount = Convert.ToInt32(PhotonNetwork.CurrentRoom.GetPlayer(ownerPV).CustomProperties["Kills"]);
            
            hash.Add("Kills", killCount + 1);
            PhotonNetwork.CurrentRoom.GetPlayer(ownerPV).SetCustomProperties(hash);
        }
    }

    [PunRPC]
    public void RPC_explode(int photonViewId)
    {
        //Call the explode function on all the machines through the network
        PhotonView.Find(photonViewId).GetComponent<PlayerDeath>().explode();
    }

    public void HealthIn()
    {
        if (currentHealth < 50.0f)
            currentHealth += 50.0f;
        else
            currentHealth = maxHealth;
    }

}
