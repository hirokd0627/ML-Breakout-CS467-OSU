// Simple test MonoBehaviour to exercise code formatter
public class mvilla_test : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 direction = Vector3.right;

    private int counter = 0;

    void Start()
    {
        Debug.Log("mvilla-test Start");
    }

    void Update()
    {
        // move object
        transform.position += direction * speed * Time.deltaTime;

        if (++counter % 60 == 0)
        {
            Debug.Log($"Moved {counter} frames. Position: {transform.position}");
        }
    }

    public int Add(int a, int b)
    {
        return a + b;
    }
}
