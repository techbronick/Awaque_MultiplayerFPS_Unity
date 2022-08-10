using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class PickupSpawnPoint : MonoBehaviour
{
    public bool empty;
    public bool weaponSpawned;

    private RaycastHit objectHit;

    void Start()
    {
        empty = false;
        weaponSpawned = true;
    }

    void Update()
    {
        //If spawn point is not empty and weaponSpawned bool is true
        if (!empty && weaponSpawned)
            //Check for pickupble presence
            rayHitCheck();

        //If spawn point is empty and weaponSpawned bool is false
        if (empty && !weaponSpawned)
        {
            //Start the spawnTimer coroutine
            StartCoroutine(spawnTimer());
            empty = false;
        }
    }
    IEnumerator spawnTimer()
    {
        //A timer coroutine which will spawn randomly a pickupble at the empty pickup spawn point, after 15 seconds
        yield return new WaitForSeconds(15f);
        string[] pickupbles = new string[] { "Weapon_Blaster 2", "Weapon_Launcher 1", "Weapon_Shotgun 1", "Pickup_Health" };
        PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", pickupbles[Random.Range(0, pickupbles.Length)]), new Vector3(this.transform.position.x, this.transform.position.y + 0.8f, this.transform.position.z), Quaternion.identity);
        weaponSpawned = true;
    }

    void rayHitCheck()
    {
        //Creates a ray which will look up and check if a pickupble is present
        Vector3 up = transform.TransformDirection(Vector3.up);
        Debug.DrawRay(transform.position, up * 2, Color.green);
        if (Physics.Raycast(transform.position, up, out objectHit, 2))
        {
            return;
        }
        else
        {
            empty = true;
            weaponSpawned = false;
        }
    }
}
