using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RoationAxes
    {
        XandY,
        X, 
        Y,
    }
    public RoationAxes axes = RoationAxes.XandY;
    public float rotationSpeedHor = 5.0f;
    public float rotationSpeedVer = 5.0f;

    public float maxVert = 45.0f;
    public float minVert = -45.0f;

    private float rotationX = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (axes == RoationAxes.XandY)
        {
            rotationX -= Input.GetAxis("Mouse Y") * rotationSpeedVer;
            rotationX = Mathf.Clamp(rotationX, minVert, maxVert);


            float delta = Input.GetAxis("Mouse X") * rotationSpeedHor;
            float rotationY = transform.localEulerAngles.y + delta;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);
        }
        else if (axes == RoationAxes.X)
        {
            transform.Rotate(0,Input.GetAxis("Mouse X") * rotationSpeedHor, 0);
        }
        else if (axes == RoationAxes.Y)
        {
            rotationX -= Input.GetAxis("Mouse Y") * rotationSpeedVer;
            rotationX = Mathf.Clamp(rotationX,minVert,maxVert);

            float rotationY = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3 (rotationX,rotationY,0);   
        }
    }
}
