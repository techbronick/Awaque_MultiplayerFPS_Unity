using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    PlayerSpawnPoint[] playerSpawnPoints;
    PickupSpawnPoint[] pickupSpawnPoints;

    private void Awake()
    {
        Instance = this;
        playerSpawnPoints = GetComponentsInChildren<PlayerSpawnPoint>();
        pickupSpawnPoints = GetComponentsInChildren<PickupSpawnPoint>();
    }

    public Transform GetPlayerSpawnPoint()
    {
        //returns a random Transform from all the Players Spawn Points
        return playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)].transform;
    }

    public Transform GetPickupSpawnPoint()
    {
        //returns a random Transform from all the Pickup Spawn Points
        return pickupSpawnPoints[Random.Range(0, pickupSpawnPoints.Length)].transform;
    }

    public PickupSpawnPoint[] GetPickupSpawnPoints()
    {
        //return all the Pickup Spawn Points
        return pickupSpawnPoints;
    }
}
