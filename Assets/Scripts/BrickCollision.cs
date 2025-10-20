using UnityEngine;

public class BrickCollision : MonoBehaviour
{
    void OnCollisionEnter2D()
    {
        Destroy(gameObject);
    }
}
