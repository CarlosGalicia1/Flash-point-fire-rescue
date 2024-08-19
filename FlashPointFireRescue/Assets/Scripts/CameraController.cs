using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float rotationSpeed = 500.0f;
    private Vector3 mouseWorldPosStart;
    private float zoomScale = 10.0f;
    private float zoomMin = 10f;
    private float zoomMax = 35.0f;

    // Límites del mapa
    private Vector2 panLimitX = new Vector2(10, 50); // Límite en el eje X
    private Vector2 panLimitZ = new Vector2(20, 50); // Límite en el eje Z

    // Límites de Rotación
    public float maxRotationZ = 15f;    

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, maxRotationZ);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            CamRotate();
        }


        if(!Input.GetMouseButton(2))
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(2))
        {
            Pan();
        }
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    private void CamRotate()
    {
        if(Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {

            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, horizontalInput, Space.World);


        }
    }

    private void Pan()
    {
        if(Input.GetAxis("Mouse Y") !=0 || Input.GetAxis("Mouse X") !=0)
        {
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position += mouseWorldPosDiff;

            // Limitar el movimiento de la cámara
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, panLimitX.x, panLimitX.y);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, panLimitZ.x, panLimitZ.y);

            transform.position = clampedPosition;
        }
    }

    private void Zoom(float zoomDiff)
    {
        if(zoomDiff != 0)
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomDiff * zoomScale, zoomMin, zoomMax);
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position += mouseWorldPosDiff;
        }
    }


    
}
