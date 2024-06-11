using UnityEngine;
public class Item : MonoBehaviour, IPickupable
{
    public void Pickup()
    {
        gameObject.SetActive(false);
    }
}