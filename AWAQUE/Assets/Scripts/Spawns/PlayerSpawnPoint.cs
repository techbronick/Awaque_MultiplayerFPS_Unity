using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        //Disable the mesh render which is used only for development purposes
        transform.GetChild(0).gameObject.SetActive(false);
    }

}
