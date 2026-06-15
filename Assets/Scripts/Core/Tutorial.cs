using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject tutorialUi;

    [SerializeField] private KeyCode hideTutorialKey = KeyCode.T;

    [SerializeField] private bool isOpened = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(hideTutorialKey))
        {
            if (isOpened)
            {
                tutorialUi.SetActive(false);

                isOpened = false;
            }
            else
            {
                tutorialUi.SetActive(true);

                isOpened = true;
            }
        }
    }
}
