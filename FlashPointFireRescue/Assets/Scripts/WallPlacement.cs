using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;

public class WallPlacement : MonoBehaviour
{
    public GameObject wallPrefab;       // Prefab de la pared
    public GameObject wallWindowPrefab; // Prefab de la pared con ventana
    public GameObject wallCornerPrefab; // Prefab de la esquina de la pared

    public int rows = 6; // Número de filas
    public int cols = 8; // Número de columnas

    public float cellWidth = 10f; // Ancho de cada celda
    public float cellHeight = 10f; // Altura de cada celda

    public float wallThickness = 0.1f; // Grosor de la pared

    private string matrixStrInput = @"
1100 1000 1001 1100 1001 1100 1000 1001
0100 0000 0011 0110 0011 0110 0010 0001
0100 0001 1100 1000 1000 1001 1100 1001
0110 0011 0110 0010 0010 0011 0110 0011
1100 1000 1000 1000 1001 1100 1001 1101
0110 0010 0010 0010 0011 0110 0011 0111
";

    void Start()
    {
        int[,] wallMatrix = StringToMatrix(matrixStrInput);
        PlaceWalls(wallMatrix);
       
    }

    int[,] StringToMatrix(string matrixStr) // Transformar el string a matriz de arreglos de 4 elementos
    {
        string[] lines = matrixStr.Trim().Split('\n');
        
        int[,] wallMatrix = new int[rows * cols, 4];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] elements = lines[i].Trim().Split(' ');
            for (int j = 0; j < elements.Length; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    wallMatrix[i * cols + j, k] = int.Parse(elements[j][k].ToString());
                }
            }
        }

        return wallMatrix;
    }

    void PlaceWalls(int[,] wallMatrix) // Posicionar y orientar paredes y esquinas
    {
        HashSet<string> drawnWalls = new HashSet<string>();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 cellPosition = new Vector3(row * cellWidth, 0, col * cellHeight);

                bool outer = false;

                // Verificar y agregar paredes arriba
                if (wallMatrix[row * cols + col, 0] == 1 && !drawnWalls.Contains((row - 1) + "," + col + ",2"))
                {
                    outer  = (row == 0) ? true : false;

                    CreateWall(cellPosition + new Vector3(0, 0, 0), 90,outer);
                    drawnWalls.Add(row + "," + col + ",0");
                }

                // Verificar y agregar paredes izquierda
                if (wallMatrix[row * cols + col, 1] == 1 && !drawnWalls.Contains(row + "," + (col - 1) + ",3"))
                {
                    outer = (col == 0) ? true : false;

                    CreateWall(cellPosition + new Vector3(cellHeight, 0, 0), 0, outer);
                    drawnWalls.Add(row + "," + col + ",1");
                }

                // Verificar y agregar paredes abajo
                if (wallMatrix[row * cols + col, 2] == 1 && !drawnWalls.Contains((row + 1) + "," + col + ",0"))
                {
                    outer = (row == rows - 1) ? true : false;

                    CreateWall(cellPosition + new Vector3(cellHeight, 0, 0), 90, outer);
                    drawnWalls.Add(row + "," + col + ",2");
                }

                // Verificar y agregar paredes derecha
                if (wallMatrix[row * cols + col, 3] == 1 && !drawnWalls.Contains(row + "," + (col + 1) + ",1"))
                {
                    outer = (col == cols - 1) ? true : false;

                    CreateWall(cellPosition + new Vector3(cellHeight, 0, cellWidth), 0, outer);
                    drawnWalls.Add(row + "," + col + ",3");
                }

                // Verificar y agregar esquina de pared superior e izquierda
                if (wallMatrix[row * cols + col, 0] == 1 && wallMatrix[row * cols + col, 1] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(0, 0, 0), 90);
                }

                // Verificar y agregar esquina de pared superior y derecha
                if (wallMatrix[row * cols + col, 0] == 1 && wallMatrix[row * cols + col, 3] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(0, 0, cellWidth), 180);
                }

                // Verificar y agregar esquina de pared inferior e izquierda
                if (wallMatrix[row * cols + col, 2] == 1 && wallMatrix[row * cols + col, 1] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(cellHeight, 0, 0), 0);
                }

                // Verificar y agregar esquina de pared inferior y derecha
                if (wallMatrix[row * cols + col, 2] == 1 && wallMatrix[row * cols + col, 3] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(cellHeight, 0, cellWidth), -90);
                }

            }
        }
    }

    void CreateWall(Vector3 position, float rotationY, bool outer) // Crear pared
    {
        if (outer == true) // Si la pared es exterior...
        {
            // Generar un número aleatorio para decidir qué objeto instanciar
            int randomIndex = Random.Range(0, 4);

            // Instanciar la pared si el numero es diferente a 0
            if (randomIndex != 0)
            {
                GameObject wall = Instantiate(wallPrefab, position, Quaternion.Euler(0, rotationY, 0));
            }
            else
            {
                GameObject wall = Instantiate(wallWindowPrefab, position, Quaternion.Euler(0, rotationY, 0));
            }
        }
        else // Si la pared no es exterior...
        {
            GameObject wall = Instantiate(wallPrefab, position, Quaternion.Euler(0, rotationY, 0));
        }

    }

    void CreateCorner(Vector3 position, float rotationY) { // Crear esquina
        GameObject corner = Instantiate(wallCornerPrefab, position, Quaternion.Euler(0, rotationY, 0));
    }
}
