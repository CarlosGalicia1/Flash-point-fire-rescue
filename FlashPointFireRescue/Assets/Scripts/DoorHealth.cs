using UnityEngine;

public class DoorHealth : MonoBehaviour
{

    public GameObject doorOpenedPrefab; // Prefab del objeto DoorOpened

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !GameManager.isGameOver) // Click derecho
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform == transform)
                {
                    SwapToOpenedDoor();
                }
            }
        }
    }

    void OnMouseDown()
    {

        if (GameManager.isGameOver) return; // No hacer nada si el juego ha terminado

        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            SetDestroy();
        }
    }

    private void SetDestroy()
    {

        GridElementManager gridElementManager = FindObjectOfType<GridElementManager>();
        if (gridElementManager != null)
        {
            gridElementManager.UpdateGridElements(GameConstants.damageCounter);
        }

        if (GameConstants.damageCounter >= GameConstants.maxDamage)
        {
            EndSimulation();
        }
        Destroy(gameObject);
    }

    public void EndSimulation()
    {
        return;
    }

    private void SwapToOpenedDoor()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        Instantiate(doorOpenedPrefab, position, rotation);
        Destroy(gameObject); // Destruye el objeto Door actual
    }
}
