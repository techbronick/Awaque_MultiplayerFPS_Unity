using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class BulletScript : MonoBehaviour
{
    public Rigidbody rb;

    public GameObject explosion;

    public LayerMask whatToDamage;

    public int explosionDamage;
    public int photonViewID;

    public float explosionRange;
    public float maxLifetime;

    private PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {

        //Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        if (explosionRange >= 2)
        {
            //Check for enemies 
            Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatToDamage);
            for (int i = 0; i < enemies.Length; i++)
            {
                //Call the RPC_TakeDamage for the enemies in the explosion area
                enemies[i].GetComponent<PhotonView>().RPC("RPC_TakeDamage", RpcTarget.All, enemies[i].GetComponent<PhotonView>().ViewID, explosionDamage);

                //Create a Hashtable which will store in enemies custom properties the information about the player who shot them last
                Hashtable hash = new Hashtable();
                hash.Add("LastBulletPhotonViewID", photonViewID);
                enemies[i].GetComponent<PhotonView>().Owner.SetCustomProperties(hash);
            }


        }
        Destroy(this.gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //If collides with other bullets ignore
        if (collision.collider.CompareTag("Bullet")) return;

        //If collides with an Enemy
        if (collision.gameObject.layer == 9)
        {
            //Call the RPC_TakeDamage on the enemy with which the collision happened
            collision.transform.GetComponent<PhotonView>().RPC("RPC_TakeDamage", RpcTarget.All, collision.transform.GetComponent<PhotonView>().ViewID, explosionDamage);

            //Create a Hashtable which will store in enemies custom properties the information about the player who shot them last
            Hashtable hash = new Hashtable();
            hash.Add("LastBulletPhotonViewID", photonViewID);
            collision.transform.GetComponent<PhotonView>().Owner.SetCustomProperties(hash);
        }
        Explode();
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<SphereCollider>().material = physics_mat;
    }

    private void OnDrawGizmosSelected()
    {
        // Just to visualize the explosion range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
