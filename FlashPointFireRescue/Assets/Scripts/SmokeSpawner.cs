using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeSpawner : MonoBehaviour
{
    public GameObject smokePrefab;          // Prefab del humo
    public GameObject firePrefab;           // Prefab del fuego
    // WallPlacement wall = FindObjectOfType<WallPlacement>();        
    public float cellWidth = 10f;           // Ancho de cada celda
    public float cellHeight = 10f;          // Altura de cada celda
    public int rows = 6;                    // Numero de filas
    public int cols = 8;                    // Numero de columnas
    
    private Dictionary<Vector2Int, string> houseFire = new Dictionary<Vector2Int, string>();

    private string matrixStrInput = @"
    2 2
    2 3
    3 2
    3 3 
    3 4
    3 5
    4 4
    5 6
    5 7
    6 6
    ";


    // Start is called before the first frame update
    void Start()
    {
        int[,] fireMatrix = StringToMatrix(matrixStrInput);
        float x, y;

        for (int i = 0; i < fireMatrix.GetLength(0); i++)
        {
            Vector2Int gridPosition = new Vector2Int(fireMatrix[i, 0], fireMatrix[i, 1]);
            houseFire.Add(gridPosition, "Fire");
            x = ((gridPosition.x - 1) * cellWidth) + (cellWidth / 2);
            y = ((gridPosition.y - 1) * cellHeight) + (cellHeight / 2);
            SpawnFire(gridPosition, x, y);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.F))
        {
            int randomX = Random.Range(1, rows);
            int randomZ = Random.Range(1, cols);
            
            //int randomX = 2;
            //int randomZ = 1;

            Vector2Int gridPosition = new Vector2Int(randomX, randomZ);

            while (houseFire.ContainsKey(gridPosition))
            {
                randomX = Random.Range(1, rows);
                randomZ = Random.Range(1, cols);
                gridPosition = new Vector2Int(randomX, randomZ);
            }

            float x = ((randomX - 1) * cellWidth) + (cellWidth / 2);
            float z = ((randomZ - 1) * cellHeight) + (cellHeight / 2);

            Debug.Log("X: " + randomX + " Z: " + randomZ);

            if (houseFire.ContainsKey(gridPosition) && houseFire[gridPosition] == "Smoke")
            {
                SpawnFire(gridPosition, x, z);
            }
            else
            {
                if (checkFireAround(gridPosition))
                {
                    SpawnFire(gridPosition, x, z);
                }
                else
                {
                    SpawnSmoke(gridPosition, x, z);
                }
            }   
        }
    }

    bool checkFireAround(Vector2Int gridPosition)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int newGridPosition = gridPosition + direction;
            if (houseFire.ContainsKey(newGridPosition) && houseFire[newGridPosition] == "Fire") return true;
        }

        return false;
    }

    int[,] StringToMatrix(string matrixStr) // Transformar el string a matriz de arreglos de 4 elementos
    {
        string[] lines = matrixStr.Trim().Split('\n');

        int[,] fireMatrix = new int[lines.Length, 2];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] elements = lines[i].Trim().Split(' ');
            for (int j = 0; j < elements.Length; j++)
            {
                fireMatrix[i, j] = int.Parse(elements[j]);
            }
        }
        
        return fireMatrix;
    }

    public void SpawnSmoke(Vector2Int gridPosition, float x, float z)
    {
        Vector3 smokePosition = new Vector3(x, 2f, z);

        Instantiate(smokePrefab, smokePosition, Quaternion.identity);

        houseFire[gridPosition] = "Smoke";
    }

    public void SpawnFire(Vector2Int gridPosition, float x, float z)
    {
        if (houseFire.ContainsKey(gridPosition) && houseFire[gridPosition] == "Smoke")
        {
            Vector3 smokePosition = new Vector3(x, 2f, z);
            GameObject smokeObject = null;

            // Iterate through all smoke objects to find the one at the specific position
            GameObject[] smokeObjects = GameObject.FindGameObjectsWithTag("Smoke");
            foreach (GameObject obj in smokeObjects)
            {
                if (obj.transform.position == smokePosition)
                {
                    smokeObject = obj;
                    break;
                }
            }

            if (smokeObject != null)
            {
                Destroy(smokeObject);
            }
        }
        
        Vector3 spawnPosition = new Vector3(x, 2f, z);
        
        Instantiate(firePrefab, spawnPosition, Quaternion.identity);

        houseFire[gridPosition] = "Fire";
    }
}
