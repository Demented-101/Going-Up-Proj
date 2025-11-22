using UnityEngine;

public class Decorator : MonoBehaviour
{
    public GenObj generator;
    public bool hasDecorated;


    private void Start()
    {
        gameObject.tag = "Decorator";
    }

    public bool AttemptDecorate()
    {
        if (hasDecorated)
        {
            return false;
        }
        else
        {
            hasDecorated = true;
            Decorate();
            return true;
        }
    }

    public virtual void Decorate()
    {
    }
}
