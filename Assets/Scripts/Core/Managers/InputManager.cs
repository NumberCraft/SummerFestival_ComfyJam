using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager i;

    private bool submitPressed = false;

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
            return;
        }

        i = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            submitPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonUp(0))
        {
            submitPressed = false;
        }
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

}
