using UnityEngine;

public class RandomWallPlacement : MonoBehaviour
{
    public GameObject wall;        // Referencia al prefab del Wall
    public GameObject wallWindow;  // Referencia al prefab del WallWindow

    private void Start()
    {
        // Definir la posición donde se colocará el objeto
        Vector3 position = new Vector3(0, 0, 0);

        // Definir la rotación del objeto
        Quaternion rotation = Quaternion.Euler(0, 90, 0);

        // Generar un número aleatorio para decidir qué objeto instanciar
        int randomIndex = Random.Range(0, 2);

        // Instanciar el objeto correspondiente en la posición especificada
        if (randomIndex == 0)
        {
            Instantiate(wall, position, rotation);
        }
        else
        {
            Instantiate(wallWindow, position, rotation);
        }
    }
}
