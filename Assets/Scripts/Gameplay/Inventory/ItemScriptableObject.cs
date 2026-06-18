using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/ItemSO", order = 3)]
public class ItemScriptableObject : ScriptableObject
{
    public string Name;
    public ItemType Type;
    //public GameObject ItemPrefab;
    //public GameObject equipedItemPrefab;
    //public Sprite ItemImage;
}

public enum ItemType
{
    Ticket,
    Key,
}