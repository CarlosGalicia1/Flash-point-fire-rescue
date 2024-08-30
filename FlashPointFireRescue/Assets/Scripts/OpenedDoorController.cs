using UnityEngine;

public class OpenedDoorController : MonoBehaviour
{
    public GameObject doorPrefab; // Prefab del objeto Door

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform == transform)
                {
                    SwapToClosedDoor();
                }
            }
        }
    }

    private void SwapToClosedDoor()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        Instantiate(doorPrefab, position, rotation);
        Destroy(gameObject); // Destruye el objeto DoorOpened actual
    }
}
