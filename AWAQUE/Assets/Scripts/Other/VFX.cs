using UnityEngine;
using Photon.Pun;

public class VFX : MonoBehaviour
{
    [SerializeField]
    private float duration;
    private void Update()
    {
        //Simple script to destroy an VFX after a se amount of time
        Destroy(gameObject, duration);
    }
}
