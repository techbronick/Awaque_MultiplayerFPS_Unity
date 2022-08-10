using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class WeaponScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject muzzleFlash;

    public float shootForce, upwardForce;

    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public float recoilForce;

    public int magazineSize, bulletsPerTap;

    public bool allowInvoke = true;
    public bool allowButtonHold;

    public int bulletsLeft, bulletsShot;

    public Rigidbody playerRb;


    public Camera fpsCam;

    public Transform attackPoint;

    public TextMeshProUGUI ammunitionDisplay;

    public AudioSource shotSFX;

    public Sprite sprite;
    public Sprite crosshair;

    public Color crosshairColor;

    private bool shooting, readyToShoot, reloading;

    private PhotonView PV;

    private Vector3 directionWithSpread;



    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
   
    private void Update()
    {
        if (PV.IsMine)
        {
            MyInput();
        }
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            OnShoot();
        }
    }

    private void Shoot(float spreadX, float spreadY)
    {
        readyToShoot = false;

        shotSFX.Play(0);

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;



        //Calculate new direction with spread
        directionWithSpread = directionWithoutSpread + new Vector3(spreadX, spreadY, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
                                                                                                   //Rotate bullet to shoot direction
        if (PV.IsMine)
        {
            //If photon view is mine set current weapon's layer to LocalWeapon
            foreach (var kid in currentBullet.transform.GetComponentsInChildren(typeof(Transform), true))
            {
                kid.gameObject.layer = 10;
            }
        }
        if (!PV.IsMine)
        {
            //If photon view is mine set current weapon's layer to Weapon
            foreach (var kid in currentBullet.transform.GetComponentsInChildren(typeof(Transform), true))
            {
                kid.gameObject.layer = 7;
            }
        }

        //Attach currents player Actor Number to the photonViewID of the current created bullet
        currentBullet.GetComponent<BulletScript>().photonViewID = PV.OwnerActorNr;

        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);


        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("OnShoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    void OnShoot()
    {
        //Calculating the spread values before calling the RPC_Shoot function, so everyone recieves the same values
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Call the RPC_Shoot function through the network
        PV.RPC("RPC_Shoot", RpcTarget.All, PV.ViewID, x, y);
    }

    public void ammoIn()
    {
        bulletsLeft = magazineSize;
    }

    [PunRPC]
    void RPC_Shoot(int weaponId, float x, float y, PhotonMessageInfo info)
    {
        //Rpc function used to call the shooting function through the network (sends random values for the spread)
        PhotonView.Find(weaponId).GetComponent<WeaponScript>().Shoot(x, y);
    }

}
