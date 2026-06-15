using UnityEngine;

public interface IWaterable
{
    void Water(float waterAmount);
    void FullyWatered();
    bool IsFullyWatered();
}
