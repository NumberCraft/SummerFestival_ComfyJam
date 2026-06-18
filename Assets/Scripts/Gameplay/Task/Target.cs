using UnityEngine;

public class Target : MonoBehaviour, IBlobHitable
{
    public DamSystem dam { get; set; }

    private bool isHitted = false;

    [ContextMenu("Hit")]
    public void Hit()
    {
        if (!isHitted)
        {
            isHitted = true;

            dam.AddShottedTarget();

            Debug.Log("target hit");
        }
    }
}
