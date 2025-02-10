using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform LookAt;
    public int MouseButton;
    public float StartDistance;
    public float StartRotation;
    public float MaxDistance;
    public float MinDistance;

    public float SensetivityX;
    public float SensetivityY;
    public float ScrollSensetivity;

    public float StartVerticalAngle;
    public float VerticalAngleMin;
    public float VerticalAngleMax;

    public float mouseX;
    private float mouseY;
    private float Distance;

    private void Start()
    {
        VerticalAngleMin -= StartVerticalAngle;
        VerticalAngleMax -= StartVerticalAngle;

        Distance = StartDistance;

        mouseX = StartRotation;
    }

    private void Update()
    {
        handlePosition();
    }

    private void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0, -Distance);
        Quaternion rotation = Quaternion.Euler(StartVerticalAngle + mouseY, mouseX, 0);
        transform.position = LookAt.position + rotation * direction;
        transform.LookAt(LookAt.position);
    }

    private void handlePosition()
    {
        if (Input.GetMouseButton(MouseButton))
        {
            mouseX += Input.GetAxis("Mouse X") * SensetivityX * Time.deltaTime;
            mouseY -= Input.GetAxis("Mouse Y") * SensetivityY * Time.deltaTime;
            mouseY = Mathf.Clamp(mouseY, VerticalAngleMin, VerticalAngleMax);
        }
        Distance -= Input.GetAxis("Mouse ScrollWheel") * ScrollSensetivity * Time.deltaTime;
        Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
    }
}
