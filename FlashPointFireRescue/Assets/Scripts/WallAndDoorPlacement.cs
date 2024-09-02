using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class WallAndDoorPlacement : MonoBehaviour
{
    public GameObject questionPrefab;
    [SerializeField] private Vector3 poiRotation = Vector3.zero;
    public GameObject wallPrefab;       // Prefab de la pared
    public GameObject wallWindowPrefab; // Prefab de la pared con ventana
    public GameObject wallCornerPrefab; // Prefab de la esquina de la pared
    public GameObject doorWallPrefab; // Prefab de la puerta 
    public GameObject smokePrefab;      // Prefab del humo
    public GameObject firePrefab;       // Prefab del fuego
    private Dictionary<Vector2Int, Tile> tileDict; // Matriz de las paredes y puertas
    private HashSet<string> drawnWalls = new HashSet<string>();
    private float floatSpeed = 1.5f;
    private float floatAmplitude = 1.8f;

    public void setDictionary(Dictionary<Vector2Int, Tile> tileDict)
    {
        this.tileDict = tileDict;
    }
    
    public void ReceiveData(string matrixStr)
    {
        bool isPair = true;
        string[] value;
        Vector2Int tilePos = new Vector2Int();
        Dictionary<Vector2Int, Tile> tileDict = new Dictionary<Vector2Int, Tile>();
        matrixStr = matrixStr.Trim('{', '}');
        string[] objectsMatrix = matrixStr.Split(new[] { "\"" }, System.StringSplitOptions.None);
        
        foreach (string objectMatrix in objectsMatrix)
        {
            if (objectMatrix != "")
            {
                if (isPair)
                {
                    isPair = false;
                    string[] key = objectMatrix.Split(',');
                    tilePos = new Vector2Int(int.Parse(key[0]), int.Parse(key[1]));
                    Debug.Log("TilePos: " + tilePos);
                }
                else
                {
                    isPair = true;
     
                    if (tilePos.x == 6 && tilePos.y == 8)
                    {
                        value = objectMatrix.Substring(2, objectMatrix.Length-2).Split(',');
                    }
                    else
                    {
                        value = objectMatrix.Substring(3, objectMatrix.Length-5).Split(',');

                    }

                    int top;
                    if (value[0].Length < 1)
                    {
                        top = int.Parse(value[0]);
                    }
                    else
                    {
                        top = int.Parse(value[0].TrimStart('['));
                    }
                    
                    int left = int.Parse(value[1]);
                    int bottom = int.Parse(value[2]);
                    int right = int.Parse(value[3]);
                    bool isOpen = bool.Parse(value[4]);
                    int topHealth = int.Parse(value[5]);
                    int leftHealth = int.Parse(value[6]);
                    int bottomHealth = int.Parse(value[7]);
                    int rightHealth = int.Parse(value[8]);
                    int fireStatus = int.Parse(value[9]);
                    bool hasPOI = bool.Parse(value[10]);
                    int numberVictims = int.Parse(value[11]);
                    List<int> fireFighters = new List<int>();

                    if (value[12].Trim() != "[]")
                    {   
                        bool isInside = false;
                        for (int i = 12; i < value.Length; i++)
                        {
                            foreach (char c in value[i])
                            {
                                if (Char.IsDigit(c))
                                {
                                    fireFighters.Add(int.Parse(c.ToString()));
                                }
                            }
                        }

                    }

                    Tile tile = new Tile(top, left, bottom, right, isOpen, topHealth, leftHealth, bottomHealth, rightHealth, fireStatus, hasPOI, numberVictims, fireFighters);
                    tileDict.Add(tilePos, tile);
                }
            }
            
        }
    }
    
    public void PlaceWalls()
    {
        foreach (KeyValuePair<Vector2Int, Tile> kvp in tileDict)
        {
            Vector2Int tilePos = kvp.Key;
            Tile tile = kvp.Value;
            int positionX = tilePos.x - 1;
            int positionY = tilePos.y - 1;
            float rotationY = 0;
            Vector3 doorPosition;
            Vector3 cellPosition = new Vector3(positionX * GameConstants.cellWidth, 0, positionY * GameConstants.cellHeight);

            bool outer = false;

            Debug.Log("TilePos: " + tilePos); 

            // Verificar y agregar paredes arriba
            if (!drawnWalls.Contains((positionX - 1) + "," + positionY + ",2") && !drawnWalls.Contains(positionX + "," + positionY + ",0"))
            {
                if (tile.getWall().getTop() == 1)
                {
                    outer  = (positionX == 0) ? true : false;
                    CreateWall(cellPosition + new Vector3(0, 0, GameConstants.cellWidth/2), 90,outer);
                    drawnWalls.Add(positionX + "," + positionY + ",0");
                    Debug.Log("Create top wall at: " + (positionX + 1) + "," + (positionY + 1));
                }
                else if (tile.getWall().getTop() == 2)
                {
                    positionX = positionX + 1;
                    positionY = positionY + 1;
                    if (positionX == 1)
                    {
                        rotationY = 90f;
                        doorPosition = new Vector3((positionX - 1) * GameConstants.cellWidth, 0, (positionY * GameConstants.cellHeight) - 5);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",0");
                    }
                    else
                    {
                        rotationY = 90f;
                        doorPosition = new Vector3(positionX * GameConstants.cellWidth, 0, (positionY * GameConstants.cellHeight) - 5);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",2");
                    }
                    CreateDoor(doorPosition, rotationY);
                    Debug.Log("Create top door at: " + (positionX) + "," + (positionY));
                }
            }

            // Verificar y agregar paredes izquierda
            if (!drawnWalls.Contains(positionX + "," + (positionY - 1) + ",3") && !drawnWalls.Contains(positionX + "," + positionY + ",1"))
            {
                if (tile.getWall().getLeft() == 1)
                {
                    outer = (positionY == 0) ? true : false;
                    CreateWall(cellPosition + new Vector3(GameConstants.cellHeight/2, 0, GameConstants.wallThickness), 0, outer);
                    drawnWalls.Add(positionX + "," + positionY + ",1");
                    Debug.Log("Create left wall at: " + (positionX + 1) + "," + (positionY + 1));
                }
                else if (tile.getWall().getLeft() == 2)
                {
                    positionX = positionX + 1;
                    positionY = positionY + 1;
                    rotationY = 0f;

                    if (positionY == 1)
                    {
                        doorPosition = new Vector3((positionX * GameConstants.cellWidth) - 5, 0, (positionY - 1) * GameConstants.cellHeight + GameConstants.wallThickness);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",1");
                    }
                    else
                    {
                        doorPosition = new Vector3((positionX * GameConstants.cellWidth) - 5, 0, positionY * GameConstants.cellHeight + GameConstants.wallThickness);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",3");
                    }
                    
                    CreateDoor(doorPosition, rotationY);
                    Debug.Log("Create left door at: " + (positionX) + "," + (positionY));
                }
            }

            // Verificar y agregar paredes abajo
            if (!drawnWalls.Contains((positionX + 1) + "," + positionY + ",0") && !drawnWalls.Contains(positionX + "," + positionY + ",2"))
            {
                if (tile.getWall().getBottom() == 1)
                {
                    outer = (positionX == GameConstants.rows - 1) ? true : false;
                    CreateWall(cellPosition + new Vector3(GameConstants.cellHeight, 0, GameConstants.cellWidth/2), 90, outer);
                    drawnWalls.Add(positionX + "," + positionY + ",2");
                    Debug.Log("Create bottom wall at: " + (positionX + 1) + "," + (positionY + 1));
                }
                else if (tile.getWall().getBottom() == 2)
                {
                    positionX = positionX + 1;
                    positionY = positionY + 1;

                    if (positionX == GameConstants.rows)
                    {
                        rotationY = -90f;
                        doorPosition = new Vector3((positionX * GameConstants.cellWidth) - 1, 0, (positionY * GameConstants.cellHeight) - 5);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",2");
                    }
                    else
                    {
                        rotationY = 90f;
                        doorPosition = new Vector3((positionX * GameConstants.cellWidth), 0, (positionY * GameConstants.cellHeight) - 5);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",2");
                    }
                    CreateDoor(doorPosition, rotationY);

                    Debug.Log("Create bottom door at: " + (positionX) + "," + (positionY));
                }   
            }

            // Verificar y agregar paredes derecha
            if (!drawnWalls.Contains(positionX + "," + (positionY + 1) + ",1") && !drawnWalls.Contains(positionX + "," + positionY + ",3"))
            {
                if (tile.getWall().getRight() == 1)
                {
                    outer = (positionY == GameConstants.cols - 1) ? true : false;
                    CreateWall(cellPosition + new Vector3(GameConstants.cellHeight/2, 0, GameConstants.cellWidth + GameConstants.wallThickness), 0, outer);
                    drawnWalls.Add(positionX + "," + positionY + ",3");
                    Debug.Log("Create right wall at: " + (positionX + 1) + "," + (positionY + 1));
                }
                else if (tile.getWall().getRight() == 2)
                {
                    positionX = positionX + 1;
                    positionY = positionY + 1;
                    
                    if (positionY == GameConstants.cols)
                    {
                        rotationY = -180f;
                        doorPosition = new Vector3((positionX * GameConstants.cellWidth) - 5, 0, positionY * GameConstants.cellHeight - 1 + GameConstants.wallThickness);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",3");
                    }
                    else
                    {
                        rotationY = 0f;
                        doorPosition = new Vector3((positionX * GameConstants.cellWidth) - 5, 0, positionY * GameConstants.cellHeight + GameConstants.wallThickness);
                        drawnWalls.Add((positionX - 1) + "," + (positionY - 1) + ",3");
                    }
                    CreateDoor(doorPosition, rotationY);
                    Debug.Log("Create right door at: " + (positionX) + "," + (positionY));
                }
            }

            if (tile.getFireStatus() != 0)
            {
                float x = ((positionX) * GameConstants.cellWidth) + (GameConstants.cellWidth / 2);
                float z = ((positionY) * GameConstants.cellHeight) + (GameConstants.cellHeight / 2);
                if (tile.getFireStatus() == 1)
                {
                    SpawnSmoke(x, z);
                }
                else if (tile.getFireStatus() == 2)
                {
                    SpawnFire(x, z);
                }
            }

            if (tile.getHasPOI())
            {
                CreatePOI(positionX + 1, positionY + 1);
            }

            CheckCorners(tile, cellPosition);

        }
    }

    void CheckCorners(Tile tile, Vector3 cellPosition)
    {
        // Verificar y agregar esquina de pared superior e izquierda
        if (tile.getWall().getTop() == 1 && tile.getWall().getLeft() == 1)
        {
            CreateCorner(cellPosition + new Vector3(-GameConstants.wallThickness, GameConstants.cellHeight/2, 0), 0);
        }

        // Verificar y agregar esquina de pared superior y derecha
        if (tile.getWall().getTop() == 1 && tile.getWall().getRight() == 1)
        {
            CreateCorner(cellPosition + new Vector3(-GameConstants.wallThickness, GameConstants.cellHeight / 2, GameConstants.cellWidth), 0);
        }

        // Verificar y agregar esquina de pared inferior e izquierda
        if (tile.getWall().getBottom() == 1 && tile.getWall().getLeft() == 1)
        {
            CreateCorner(cellPosition + new Vector3(GameConstants.cellHeight - GameConstants.wallThickness, GameConstants.cellHeight/2, 0), 0);
        }

        // Verificar y agregar esquina de pared inferior y derecha
        if (tile.getWall().getBottom() == 1 && tile.getWall().getRight() == 1)
        {
            CreateCorner(cellPosition + new Vector3(GameConstants.cellHeight - GameConstants.wallThickness, GameConstants.cellHeight / 2, GameConstants.cellWidth), 0);
        }
    }

    void SpawnSmoke(float x, float z)
    {
        Vector3 spawnPosition = new Vector3(x, 2f, z);
        Instantiate(smokePrefab, spawnPosition, Quaternion.identity);
    }
    
    void SpawnFire(float x, float z)
    {
        Vector3 smokePosition = new Vector3(x, 2f, z);
        GameObject smokeObject = null;

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

        Vector3 spawnPosition = new Vector3(x, 2f, z);
        Instantiate(firePrefab, spawnPosition, Quaternion.identity);
    }
    
    void CreateDoor(Vector3 position, float rotationY)
    {
        GameObject doorWall = Instantiate(doorWallPrefab, position, Quaternion.Euler(0, rotationY, 0));
    }

    void CreateWall(Vector3 position, float rotationY, bool outer) // Crear pared
    {
        if (outer == true) // Si la pared es exterior...
        {
            // Generar un n�mero aleatorio para decidir qu� objeto instanciar
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

    void CreatePOI(int file, int column)
    {
        float x = 7f + ((file - 1f) * (10f));
        float y = 2f;
        float z = 5f + ((column - 1f) * 10f);

        Vector3 position = new Vector3(x, y, z);
        GameObject poi = Instantiate(questionPrefab, position, Quaternion.Euler(poiRotation));
        
        poi.transform.SetParent(transform);
        StartCoroutine(FloatPOI(poi.transform));
    }

    IEnumerator FloatPOI(Transform poiTransform)
    {
        Vector3 startPosition = poiTransform.position;
        while (true)
        {
            // Calculate the new Y position using a sine wave
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

            // Update the POI's position
            poiTransform.position = new Vector3(poiTransform.position.x, newY, poiTransform.position.z);

            yield return null;
        }
    }
}
