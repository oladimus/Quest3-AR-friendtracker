using UnityEngine;

public class CustomGravit : MonoBehaviour
{
    public float gravityScale = 0.25f;
    private Rigidbody rb;
    private Vector3 gravity = Physics.gravity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(gravity * gravityScale, ForceMode.Acceleration);
    }
}
