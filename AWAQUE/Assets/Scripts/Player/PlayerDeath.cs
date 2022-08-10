using UnityEngine;
public class PlayerDeath : MonoBehaviour
{
    public int cubesInRow = 1;

    public float cubeSize = 0.1f;
    public float explosionForce = 10f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;
    public float destroyDelay = 1.5f;

    private float cubesPivotDistance;

    private Vector3 cubesPivot;

    void Start()
    {
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
    }


    public void explode()
    {
        //make the current object(Player) disappear
        foreach (var kid in GetComponentsInChildren(typeof(Transform), true))
        {
            kid.gameObject.SetActive(false);
        }

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    createPiece(x, y, z);
                }
            }
        }


        //get explosion position
        Vector3 explosionPos = transform.position;
        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders)
        {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }
        Invoke("Respawn", 3f);
    }

    void createPiece(int x, int y, int z)
    {

        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //set piece position and scale
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;
        Destroy(piece, destroyDelay);
    }


    void Respawn()
    {
        //selects a random spawn point
        Transform spawnPoint = SpawnManager.Instance.GetPlayerSpawnPoint();

        //restore health to max value
        GetComponent<PlayerHealth>().currentHealth = GetComponent<PlayerHealth>().maxHealth;

        //set player position
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        //make the player capsule apear
        foreach (var kid in GetComponentsInChildren(typeof(Transform), true))
        {
            kid.gameObject.SetActive(true);
        }
    }

}
