using UnityEngine;
public class Weapon : MonoBehaviour, IPickupable
{
    public void Pickup()
    {
        gameObject.SetActive(false);
    }
}