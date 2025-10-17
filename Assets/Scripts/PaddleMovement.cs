using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    // Serialized values, max and min set in inspector
    public float speed = 18;
    public float maxX = 7f;
    public float minX = 7f;

    float horizMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        horizMovement = Input.GetAxis("Horizontal");
        if (
            (horizMovement > 0 && transform.position.x < maxX)
            || (horizMovement < 0 && transform.position.x > minX)
        )
        {
            transform.position += Vector3.right * horizMovement * speed * Time.deltaTime;
        }
    }
}
