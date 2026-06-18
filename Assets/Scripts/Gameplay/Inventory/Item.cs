using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private int Id;
    public int id { get { return Id; } set { Id = value; } }

    [Header("References")]
    [SerializeField] private ItemScriptableObject itemSO;

    /*[Header("Interact Properties")]
    [SerializeField] private float pickupDistance = 5f;
    [SerializeField] private LayerMask pickupMask;

    private Transform cam;

    private RaycastHit hit;

    private void Update()
    {
        if (Camera.main == null)
            return;

        cam = Camera.main.transform;

        if (Physics.Raycast(cam.position, cam.forward, out hit, pickupDistance, pickupMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                InteractUIManager.Instance.Show(InteractType.Item);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    bool canAdd = PlayerInventory.ownerPlayerInventory.TryAddItem(itemSO);

                    if (canAdd)
                    {
                        Destroy(gameObject);

                        P2P_Manager.Instance.SendPickupItem(Camera.main.GetComponentInParent<P2P_NetworkedObject>().ObjectId, id);
                    }
                }
            }
        }
        else
        {
            InteractUIManager.Instance.Hide(InteractType.Item);
        }
    }*/

    private void PickupItem()
    {
        bool canAdd = PlayerInventory.i.TryAddItem(itemSO);

        if (canAdd)
        {
            Destroy(gameObject);
        }
    }

    public void SetItem(ItemScriptableObject item)
    {
        this.itemSO = item;
    }

    public ItemScriptableObject GetItem()
    {
        return itemSO;
    }

    public void Interact()
    {
        PickupItem();
    }

    public InteractType GetInteractType()
    {
        return InteractType.Item;
    }
}
