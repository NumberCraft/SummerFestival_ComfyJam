using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<Transform> itemSlots = new();

    [Header("Slot Properties")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color deselectedColor;

    /*private void Start()
    {
        if (PlayerInventory.ownerPlayerInventory != null)
        {
            PlayerInventory.ownerPlayerInventory.onAddItem += AddItem;
            PlayerInventory.ownerPlayerInventory.onRemoveItem += RemoveItem;
            PlayerInventory.ownerPlayerInventory.onSelectItem += SelectSlot;

            SelectSlot(0, false);
        }
    }

    public void AddItem(int index, ItemScriptableObject item)
    {
        Image itemImage = itemSlots[index].Find("background").GetChild(0).GetComponent<Image>();
        itemImage.sprite = item.ItemImage;

        itemImage.gameObject.SetActive(true);
    }

    public void RemoveItem(int index, ItemScriptableObject item)
    {
        Image itemImage = itemSlots[index].Find("background").GetChild(0).GetComponent<Image>();

        itemImage.gameObject.SetActive(false);
    }

    public void SelectSlot(int index, bool exist)
    {
        foreach (Transform slot in itemSlots)
        {
            slot.Find("background").GetComponent<Image>().color = deselectedColor;
        }

        itemSlots[index].Find("background").GetComponent<Image>().color = selectedColor;
    }*/
}
