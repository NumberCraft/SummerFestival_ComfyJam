using UnityEngine;

public class FestivalTrigger : MonoBehaviour
{
    [SerializeField] private int festivalIndex = 2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GoToFestival();
        }
    }

    private void GoToFestival()
    {
        GameManager.i.StartGameByIndex(festivalIndex);
    }
}
