using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory ownerPlayerInventory;

    //public Action<int, bool> onSelectItem;
    //public Action<int, ItemScriptableObject> onAddItem;
    //public Action<int, ItemScriptableObject> onRemoveItem;

    //[SerializeField] private int inventoryCapacity = 4;

    //[SerializeField] private Transform dropTransform;

    //public Dictionary<int, ItemScriptableObject> items = new();
    public List<ItemScriptableObject> items = new();
    //public int selectedItemIndex { get; private set; }

    [Header("Ticket")]
    [SerializeField] private DialogueTrigger securityDialogueTrigger;

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetItem(3);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            TryUseItem(selectedItemIndex);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //TryRemoveItem(selectedItemIndex);
            TryDropItem(selectedItemIndex);
        }
    }*/

    /*public void SetItem(int index)
    {
        selectedItemIndex = index;

        EquipItem();

        onSelectItem?.Invoke(index, items.ContainsKey(index));
    }*/

    /*public bool TryAddItem(ItemScriptableObject item)
    {
        if (items.Count < inventoryCapacity)
        {
            for (int i = 0; i < inventoryCapacity; i++)
            {
                if (!items.ContainsKey(i))
                {
                    items.Add(i, item);

                    if (i == selectedItemIndex)
                    {
                        SetItem(i);
                    }

                    onAddItem?.Invoke(i, item);

                    return true;
                }
            }
        }

        return false;
    }*/

    public bool TryAddItem(ItemScriptableObject item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);

            return true;
        }

        return false;
    }

    public void AddItem(ItemScriptableObject item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);

            if (item.Type == ItemType.Ticket)
            {
                securityDialogueTrigger.ChangeDialogueIndexTo(2);
            }
        }
    }

    /*public ItemScriptableObject TryRemoveItem(int index)
    {
        if (items.ContainsKey(index))
        {
            var itemSO = items[index];

            onRemoveItem?.Invoke(index, items[index]);

            items.Remove(index);

            DestroyEquipedItem();

            return itemSO;
        }

        return null;
    }

    public ItemScriptableObject TryDropItem(int index)
    {
        if (items.ContainsKey(index))
        {
            Debug.Log("Dropped Item");

            var itemSO = TryRemoveItem(index);

            Instantiate(itemSO.ItemPrefab, dropTransform.position, dropTransform.rotation);

            return itemSO;
        }

        return null;
    }

    private void EquipItem()
    {
        DestroyEquipedItem();

        if (!items.ContainsKey(selectedItemIndex))
            return;

        GameObject item = Instantiate(items[selectedItemIndex].equipedItemPrefab);
        meshSockets.Attach(item.transform, MeshSockets.SocketId.RightHand);
    }

    private void DestroyEquipedItem()
    {
        if (meshSockets.GetSocketWithID(MeshSockets.SocketId.RightHand) != null && meshSockets.GetItemObject(MeshSockets.SocketId.RightHand) != null)
        {
            Destroy(meshSockets.GetItemObject(MeshSockets.SocketId.RightHand));
        }
    }

    public void SetObjectID(string objectId)
    {
        this.objectId = objectId;
    }*/
}

/*public class WorldItem
{
    public int ItemID;
    public ItemScriptableObject Data;
    public Vector3 Position;
    public Quaternion Rotation;
}*/