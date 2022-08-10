using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
public class HUDScript : MonoBehaviour
{
    private PhotonView PV;

    public GameObject[] whatToHide;
    public GameObject ScoreBoard;
    public GameObject Options;

    public Transform WeaponHolder;

    public Image weaponImage;
    public Image crosshairImage;

    public Sprite defaultSprite;
    public Sprite defaultCrosshair;

    public TextMeshProUGUI ammunitionDisplay;

    public Color defaultColor;

    bool showScoreboard;
    bool options;
    private void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    private void Start()
    {
        showScoreboard = false;
    }

    private void Update()
    {
        if (!PV.IsMine) return;


        //Scoreboard and option Logic
        if (showScoreboard)
        {
            ScoreBoard.SetActive(true);
            foreach (var obj in whatToHide)
            {
                obj.gameObject.SetActive(false);
            }
        }
        else
        {
            ScoreBoard.SetActive(false);
            foreach (var obj in whatToHide)
            {
                obj.gameObject.SetActive(true);
            }
        }

        if (options)
        {
            Options.SetActive(true);
            foreach (var obj in whatToHide)
            {
                obj.gameObject.SetActive(false);
            }
        }
        else
        {
            Options.SetActive(false);
            foreach (var obj in whatToHide)
            {
                obj.gameObject.SetActive(true);
            }
        }

        //Input management
        if (options && Input.GetKeyDown(KeyCode.Q))
        {
            PhotonNetwork.LeaveRoom();
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showScoreboard = true;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            showScoreboard = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            options = true;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            options = false;
        }

        SetWeaponIconAndAmmoDisplay();

    }

    //Logic for setting the Weapon Icon and Ammunition display
    public void SetWeaponIconAndAmmoDisplay()
    {
        if (WeaponHolder.childCount != 0)
            foreach (Transform weapon in WeaponHolder)
            {
                if (weapon.gameObject.activeSelf == true)
                {
                    WeaponScript script = weapon.GetComponent<WeaponScript>();

                    crosshairImage.sprite = script.crosshair;
                    crosshairImage.color = script.crosshairColor;

                    weaponImage.sprite = script.sprite;
                    weaponImage.color = script.crosshairColor;

                    ammunitionDisplay.SetText(script.bulletsLeft / script.bulletsPerTap + " / " + script.magazineSize / script.bulletsPerTap);
                    ammunitionDisplay.color = script.crosshairColor;

                }
            }
        else
        {
            crosshairImage.sprite = defaultCrosshair;
            crosshairImage.color = defaultColor;

            weaponImage.sprite = defaultSprite;
            weaponImage.color = defaultColor;

            ammunitionDisplay.SetText(" ");
        }
    }
}
