using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 mouseWorldPosStart;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, GameConstants.maxRotationZ);
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

            float horizontalInput = Input.GetAxis("Mouse X") * GameConstants.rotationSpeed * Time.deltaTime;
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
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, GameConstants.panLimitX.x, GameConstants.panLimitX.y);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, GameConstants.panLimitZ.x, GameConstants.panLimitZ.y);

            transform.position = clampedPosition;
        }
    }

    private void Zoom(float zoomDiff)
    {
        if(zoomDiff != 0)
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomDiff * GameConstants.zoomScale, GameConstants.zoomMin, GameConstants.zoomMax);
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position += mouseWorldPosDiff;
        }
    }


    
}
