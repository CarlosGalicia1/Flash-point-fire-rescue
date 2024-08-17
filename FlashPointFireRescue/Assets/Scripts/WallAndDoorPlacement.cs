using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI.Table;

public class WallAndDoorPlacement : MonoBehaviour
{
    public GameObject wallPrefab;       // Prefab de la pared
    public GameObject wallWindowPrefab; // Prefab de la pared con ventana
    public GameObject wallCornerPrefab; // Prefab de la esquina de la pared
    public GameObject doorWallPrefab; // Prefab de la puerta 

    public int rows = 6; // Número de filas
    public int cols = 8; // Número de columnas

    public float cellWidth = 10f; // Ancho de cada celda
    public float cellHeight = 10f; // Altura de cada celda

    public float wallThickness = 0.1f; // Grosor de la pared
    private HashSet<string> drawnWalls = new HashSet<string>();

    private string matrixStrInput = @"
1100 1000 1001 1100 1001 1100 1000 1001
0100 0000 0011 0110 0011 0110 0010 0001
0100 0001 1100 1000 1000 1001 1100 1001
0110 0011 0110 0010 0010 0011 0110 0011
1100 1000 1000 1000 1001 1100 1001 1101
0110 0010 0010 0010 0011 0110 0011 0111
";

    private string doorsStrInput = @"
1 3 1 4
2 5 2 6
2 8 3 8
3 2 3 3 
4 4 5 4
4 6 4 7
6 5 6 6
6 7 6 8
";

    private string entryPointsStrInput = @"
1 6
3 1
4 8
6 3
";

    void Start()
    {
        int[,] entryMatrix = StringTOEntryMatrix(entryPointsStrInput);
        PlaceEntryPoints(entryMatrix);

        int[,] doorMatrix = StringToDoorMatrix(doorsStrInput);
        PlaceDoors(doorMatrix);


        int[,] wallMatrix = StringToMatrix(matrixStrInput);
        PlaceWalls(wallMatrix);

        foreach (var wall in drawnWalls)
        {
            Debug.Log($"Wall: {wall}");
        }

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

    int[,] StringToDoorMatrix(string doorsStr)
    {
        string[] lines = doorsStr.Trim().Split('\n');
        int[,] doorMatrix = new int[lines.Length, 4];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] elements = lines[i].Trim().Split(' ');
            for (int j = 0; j < elements.Length; j++)
            {
                doorMatrix[i, j] = int.Parse(elements[j]);
            }
        }

        return doorMatrix;
    }

    int[,] StringTOEntryMatrix(string entryStr)
    {
        string[] lines = entryStr.Trim().Split('\n');
        int[,] entryMatrix = new int[lines.Length, 2];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] elements = lines[i].Trim().Split(' ');
            for (int j = 0; j < elements.Length; j++)
            {
                entryMatrix[i, j] = int.Parse(elements[j]);
            }
        }

        return entryMatrix;
    }


    void PlaceWalls(int[,] wallMatrix) // Posicionar y orientar paredes y esquinas
    {
        

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 cellPosition = new Vector3(row * cellWidth, 0, col * cellHeight);

                bool outer = false;

                // Verificar y agregar paredes arriba
                if (wallMatrix[row * cols + col, 0] == 1 && !drawnWalls.Contains((row - 1) + "," + col + ",2") && !drawnWalls.Contains(row + "," + col + ",0"))
                {
                    outer  = (row == 0) ? true : false;
                    CreateWall(cellPosition + new Vector3(0, 0, 5), 90,outer);
                    drawnWalls.Add(row + "," + col + ",0");
                }

                // Verificar y agregar paredes izquierda
                if (wallMatrix[row * cols + col, 1] == 1 && !drawnWalls.Contains(row + "," + (col - 1) + ",3") && !drawnWalls.Contains(row + "," + col + ",1"))
                {
                    outer = (col == 0) ? true : false;
                    CreateWall(cellPosition + new Vector3(cellHeight/2, 0, 0), 0, outer);
                    drawnWalls.Add(row + "," + col + ",1");
                }

                // Verificar y agregar paredes abajo
                if (wallMatrix[row * cols + col, 2] == 1 && !drawnWalls.Contains((row + 1) + "," + col + ",0") && !drawnWalls.Contains(row + "," + col + ",2"))
                {
                    outer = (row == rows - 1) ? true : false;
                    CreateWall(cellPosition + new Vector3(cellHeight, 0, cellWidth/2), 90, outer);
                    drawnWalls.Add(row + "," + col + ",2");
                }

                // Verificar y agregar paredes derecha
                if (wallMatrix[row * cols + col, 3] == 1 && !drawnWalls.Contains(row + "," + (col + 1) + ",1") && !drawnWalls.Contains(row + "," + col + ",3"))
                {
                    outer = (col == cols - 1) ? true : false;
                    CreateWall(cellPosition + new Vector3(cellHeight/2, 0, cellWidth), 0, outer);
                    drawnWalls.Add(row + "," + col + ",3");
                }

                // Verificar y agregar esquina de pared superior e izquierda
                if (wallMatrix[row * cols + col, 0] == 1 && wallMatrix[row * cols + col, 1] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(-.5f, cellHeight/2, -.5f), 0);
                }

                // Verificar y agregar esquina de pared superior y derecha
                if (wallMatrix[row * cols + col, 0] == 1 && wallMatrix[row * cols + col, 3] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(-.5f, cellHeight / 2, cellWidth - .5f), 0);
                }

                // Verificar y agregar esquina de pared inferior e izquierda
                if (wallMatrix[row * cols + col, 2] == 1 && wallMatrix[row * cols + col, 1] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(cellHeight - .5f, cellHeight/2, -.5f), 0);
                }

                // Verificar y agregar esquina de pared inferior y derecha
                if (wallMatrix[row * cols + col, 2] == 1 && wallMatrix[row * cols + col, 3] == 1)
                {
                    CreateCorner(cellPosition + new Vector3(cellHeight - .5f, cellHeight / 2, cellWidth - .5f), 0);
                }

            }
        }
        
    }

    void PlaceDoors(int[,] doorMatrix)
    {
        int doorCount = doorMatrix.GetLength(0); // Número de filas en la matriz

        for (int i = 0; i < doorCount; i++)
        {
            int row1 = doorMatrix[i, 0]; // El -1 es para ajustar el índice
            int col1 = doorMatrix[i, 1];
            int row2 = doorMatrix[i, 2];
            int col2 = doorMatrix[i, 3];

            Vector3 doorPosition;
            float rotationY;

            if (row1 != row2)
            {
                rotationY = 90;
                doorPosition = new Vector3((row1 * cellWidth), 0, (col1 * cellHeight) -5);
                drawnWalls.Add((row1 - 1) + "," + (col1 - 1) + ",2");
            }
            else
            {
                rotationY = 0;
                doorPosition = new Vector3((row1 * cellWidth) - 5, 0, col1 * cellHeight);
                drawnWalls.Add((row1 - 1) + "," + (col1 - 1) + ",3");
            }

            CreateDoor(doorPosition, rotationY);
        }
    }

    void PlaceEntryPoints(int[,] entryMatrix)
    {
        int entryCount = entryMatrix.GetLength(0);

        for (int i = 0; i < entryCount; i++)
        {
            int row = entryMatrix[i, 0];
            int col = entryMatrix[i, 1];

            Vector3 entryPosition = new Vector3(row * cellWidth, 0, col * cellHeight);
            float rotationY = 0f;

            // Ajustar la rotación según la ubicación de la entrada
            if (row == 1)
            {
                rotationY = 90f; // Entrada superior
                entryPosition = new Vector3((row -1) * cellWidth, 0, (col * cellHeight) - 5);
                drawnWalls.Add((row-1) + "," + (col-1) + ",0");
            }
            else if (row == rows)
            {
                rotationY = -90f; // Entrada inferior
                entryPosition = new Vector3((row * cellWidth) - 1, 0, (col * cellHeight) - 5);
                drawnWalls.Add((row - 1) + "," + (col - 1) + ",2");
            }
            else if (col == 1)
            {
                rotationY = 0f; // Entrada izquierda
                entryPosition = new Vector3((row * cellWidth) - 5, 0, (col -1) * cellHeight);
                drawnWalls.Add((row - 1) + "," + (col - 1) + ",1");
            }
            else if (col == cols)
            {
                rotationY = -180f; // Entrada derecha
                entryPosition = new Vector3((row * cellWidth) - 5, 0, col * cellHeight - 1);
                drawnWalls.Add((row - 1) + "," + (col - 1) + ",3");
            }

            CreateDoor(entryPosition, rotationY);
        }
    }

    void CreateDoor(Vector3 position, float rotationY)
    {
        GameObject doorWall = Instantiate(doorWallPrefab, position, Quaternion.Euler(0, rotationY, 0));
    }

    void CreateWall(Vector3 position, float rotationY, bool outer) // Crear pared
    {
        if (outer == true) // Si la pared es exterior...
        {
            // Generar un número aleatorio para decidir qué objeto instanciar
            int randomIndex = UnityEngine.Random.Range(0, 4);

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
