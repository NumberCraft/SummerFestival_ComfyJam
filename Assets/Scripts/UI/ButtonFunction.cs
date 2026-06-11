using UnityEngine;
using UnityEngine.UI;

public class ButtonFunction : MonoBehaviour
{
    public bool disableOnce;

    /*private void Start()
    {
        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(() => PlaySound("ButtonSelect"));
        }
    }*/

    public void ButtonSelect(string soundName)
    {
        if (!disableOnce)
        {
            AudioManager.Play(soundName);
        }
        else
        {
            disableOnce = false;
        }
    }

    public void ButtonPress(string soundName)
    {
        if (!disableOnce)
        {
            AudioManager.Play(soundName);
        }
        else
        {
            disableOnce = false;
        }
    }
}
