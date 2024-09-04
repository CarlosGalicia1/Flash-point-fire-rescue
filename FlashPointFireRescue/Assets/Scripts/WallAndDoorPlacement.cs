using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using static WebClient;

public class WallAndDoorPlacement : MonoBehaviour
{
    public GameObject redAgentPrefab;   // Prefab del agente rojo
    public GameObject blueAgentPrefab;  // Prefab del agente azul
    public GameObject greenAgentPrefab; // Prefab del agente verde
    public GameObject yellowAgentPrefab; // Prefab del agente amarillo
    public GameObject whiteAgentPrefab; // Prefab del agente
    public GameObject purpleAgentPrefab; // Prefab del agente
    public GameObject questionPrefab; // Prefab del POI
    [SerializeField] private Vector3 poiRotation = Vector3.zero;
    public GameObject wallPrefab;       // Prefab de la pared
    public GameObject wallWindowPrefab; // Prefab de la pared con ventana
    public GameObject wallCornerPrefab; // Prefab de la esquina de la pared
    public GameObject doorWallPrefab; // Prefab de la puerta 
    public GameObject doorOpenedPrefab;
    public GameObject smokePrefab;      // Prefab del humo
    public GameObject firePrefab;       // Prefab del fuego
    private Dictionary<Vector2Int, Tile> tileDict; // Matriz de las paredes y puertas
    private HashSet<string> drawnWalls = new HashSet<string>();
    private float floatSpeed = 1.5f;
    private float floatAmplitude = 1.8f;

    private GameObject redAgentInstance;
    private GameObject blueAgentInstance;
    private GameObject greenAgentInstance;
    private GameObject yellowAgentInstance;
    private GameObject whiteAgentInstance;
    private GameObject purpleAgentInstance;

    private Dictionary<Vector3, GameObject> wallDict = new Dictionary<Vector3, GameObject>();
    private Dictionary<Vector3, GameObject> doorDict = new Dictionary<Vector3, GameObject>();
    private List<GameObject> destroyList = new List<GameObject> ();

    public void setDictionary(Dictionary<Vector2Int, Tile> tileDict)
    {
        this.tileDict = tileDict;
    }
    
    public void UpdateEnvironment(KeyValuePair<Vector2Int, Tile> kvp)
    {

        Vector2Int tilePos = kvp.Key;
        Tile tile = kvp.Value;
        int positionX = tilePos.x - 1;
        int positionY = tilePos.y - 1;
        float rotationY = 0;
        Vector3 doorPosition;
        Vector3 cellPosition = new Vector3(positionX * GameConstants.cellWidth, 0, positionY * GameConstants.cellHeight);

        // Verifica si la pared superior debe ser destruida o cambiar de material
        Vector3 topWallPosition = cellPosition + new Vector3(0, 0, GameConstants.cellWidth / 2);
        HandleWall(tile.getWall().getTopHealth(), topWallPosition);

        // Verifica si la pared inferior debe ser destruida o cambiar de material
        Vector3 bottomWallPosition = cellPosition + new Vector3(GameConstants.cellHeight, 0, GameConstants.cellWidth / 2);
        HandleWall(tile.getWall().getBottomHealth(), bottomWallPosition);

        // Verifica si la pared izquierda debe ser destruida o cambiar de material
        Vector3 leftWallPosition = cellPosition + new Vector3(GameConstants.cellHeight / 2, 0, GameConstants.wallThickness);
        HandleWall(tile.getWall().getLeftHealth(), leftWallPosition);

        // Verifica si la pared derecha debe ser destruida o cambiar de material
        Vector3 rightWallPosition = cellPosition + new Vector3(GameConstants.cellHeight / 2, 0, GameConstants.cellWidth + GameConstants.wallThickness);
        HandleWall(tile.getWall().getRightHealth(), rightWallPosition);

        // Verificar y manejar la puerta superior
        Vector3 topDoorPosition = cellPosition + new Vector3(0, 0, GameConstants.cellWidth / 2);
        HandleDoor(tile.getWall().getTopHealth(), topDoorPosition);
    
        // Verificar y manejar la puerta inferior
        Vector3 bottomDoorPosition = cellPosition + new Vector3(GameConstants.cellHeight, 0, GameConstants.cellWidth / 2);
        HandleDoor(tile.getWall().getBottomHealth(), bottomDoorPosition);
    
        // Verificar y manejar la puerta izquierda
        Vector3 leftDoorPosition = cellPosition + new Vector3(GameConstants.cellHeight / 2, 0, GameConstants.wallThickness);
        HandleDoor(tile.getWall().getLeftHealth(), leftDoorPosition);
    
        // Verificar y manejar la puerta derecha
        Vector3 rightDoorPosition = cellPosition + new Vector3(GameConstants.cellHeight / 2, 0, GameConstants.cellWidth + GameConstants.wallThickness);
        HandleDoor(tile.getWall().getRightHealth(), rightDoorPosition);
        
        // Reemplazar puerta cerrada por abierta si es necesario
        if (tile.getWall().getTop() == 2 && tile.getWall().getIsOpen())
        {
            ReplaceWithOpenedDoor(topDoorPosition, 90f);
        }

        if (tile.getWall().getBottom() == 2 && tile.getWall().getIsOpen())
        {
            ReplaceWithOpenedDoor(bottomDoorPosition, -90f);
        }

        if (tile.getWall().getLeft() == 2 && tile.getWall().getIsOpen())
        {
            ReplaceWithOpenedDoor(leftDoorPosition, 0f);
        }

        if (tile.getWall().getRight() == 2 && tile.getWall().getIsOpen())
        {
            ReplaceWithOpenedDoor(rightDoorPosition, 0f);
        }
        
        bool outer = false;

        Debug.Log("TilePos: " + tilePos);
        Debug.Log("Tile firefighters: " + string.Join(", ", tile.getFireFighters()));

        if (tile.getFireFighters().Count > 0)
        {
            int totalFireFighters = tile.getFireFighters().Count;
            float centerX = ((positionX) * GameConstants.cellWidth) + (GameConstants.cellWidth / 2);
            float centerZ = ((positionY) * GameConstants.cellHeight) + (GameConstants.cellHeight / 2);
            int index = 0;

            foreach (int firefighter in tile.getFireFighters())
            {
                float x, z;

                if (totalFireFighters == 1)
                {
                    // Si solo hay un firefighter, colócalo en el centro
                    x = centerX;
                    z = centerZ;
                }
                else
                {
                    // Si hay más de un firefighter, colócalos alrededor del centro
                    float angle = index * Mathf.PI * 2 / totalFireFighters; // Calcula el ángulo para la posición
                    float radius = GameConstants.cellWidth / 4f; // Radio del círculo alrededor del centro
                    x = centerX + Mathf.Cos(angle) * radius;
                    z = centerZ + Mathf.Sin(angle) * radius;
                }

                Vector3 position = new Vector3(x, 2f, z);
                GameObject agent = null;

                switch (firefighter)
                {
                    case 0:
                        if (redAgentInstance != null)
                        {
                            Destroy(redAgentInstance);
                        }
                        redAgentInstance = Instantiate(redAgentPrefab, position, Quaternion.identity);
                        break;
                    case 1:
                        if (blueAgentInstance != null)
                        {
                            Destroy(blueAgentInstance);
                        }
                        blueAgentInstance = Instantiate(blueAgentPrefab, position, Quaternion.identity);
                        break;
                    case 2:
                        if (greenAgentInstance != null)
                        {
                            Destroy(greenAgentInstance);
                        }
                        greenAgentInstance = Instantiate(greenAgentPrefab, position, Quaternion.identity);
                        break;
                    case 3:
                        if (yellowAgentInstance != null)
                        {
                            Destroy(yellowAgentInstance);
                        }
                        yellowAgentInstance = Instantiate(yellowAgentPrefab, position, Quaternion.identity);
                        break;
                    case 4:
                        if (whiteAgentInstance != null)
                        {
                            Destroy(whiteAgentInstance);
                        }
                        whiteAgentInstance = Instantiate(whiteAgentPrefab, position, Quaternion.identity);
                        break;
                    case 5:
                        if (purpleAgentInstance != null)
                        {
                            Destroy(purpleAgentInstance);
                        }
                        purpleAgentInstance = Instantiate(purpleAgentPrefab, position, Quaternion.identity);
                        break;
                }
                index++;
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

        else if (tile.getFireStatus() == 0)
        {
            float x = ((positionX) * GameConstants.cellWidth) + (GameConstants.cellWidth / 2);
            float z = ((positionY) * GameConstants.cellHeight) + (GameConstants.cellHeight / 2);
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
        }

        if (tile.getHasPOI())
        {
            CreatePOI(positionX + 1, positionY + 1);
        }

        else if (!tile.getHasPOI() && tile.getFireFighters().Count > 0 || tile.getFireStatus() == 2)
        {
            Debug.Log("Poi revelado en la posicion: " + (positionX+1) + ", " + (positionY+1));
            float x = 7f + ((positionX) * (10f));
            float y = 1.8f;
            float z = 5f + ((positionY) * 10f);

            Vector3 position = new Vector3(x, y, z);
            GameObject poi = null;

            GameObject[] pois = GameObject.FindGameObjectsWithTag("POI");
            foreach (GameObject obj in pois)
            {
                Debug.Log("Comparando POI en: " + obj.transform.position + " con: " + position);
                Debug.Log("Distancia: " + Vector3.Distance(obj.transform.position, position));
                if (Vector3.Distance(obj.transform.position, position) < 3f)
                {   
                    Debug.Log("POI encontrado en: " + (positionX + 1) + ", " + (positionY + 1));
                    destroyList.Add(obj);


                }

            }
        }

        // Verificar y agregar paredes y puertas arriba
        if (!drawnWalls.Contains((positionX - 1) + "," + positionY + ",2") && !drawnWalls.Contains(positionX + "," + positionY + ",0"))
        {
            if (tile.getWall().getTop() == 1)
            {
                outer = (positionX == 0) ? true : false;
                CreateWall(cellPosition + new Vector3(0, 0, GameConstants.cellWidth / 2), 90, outer);
                drawnWalls.Add(positionX + "," + positionY + ",0");
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
                positionX = positionX - 1;
                positionY = positionY - 1;
            }
        }

        // Verificar y agregar paredes y puertas izquierda
        if (!drawnWalls.Contains(positionX + "," + (positionY - 1) + ",3") && !drawnWalls.Contains(positionX + "," + positionY + ",1"))
        {
            if (tile.getWall().getLeft() == 1)
            {
                outer = (positionY == 0) ? true : false;
                CreateWall(cellPosition + new Vector3(GameConstants.cellHeight / 2, 0, GameConstants.wallThickness), 0, outer);
                drawnWalls.Add(positionX + "," + positionY + ",1");
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
                positionX = positionX - 1;
                positionY = positionY - 1;
            }
        }

        // Verificar y agregar paredes y puertas abajo
        if (!drawnWalls.Contains((positionX + 1) + "," + positionY + ",0") && !drawnWalls.Contains(positionX + "," + positionY + ",2"))
        {
            if (tile.getWall().getBottom() == 1)
            {
                outer = (positionX == GameConstants.rows - 1) ? true : false;
                CreateWall(cellPosition + new Vector3(GameConstants.cellHeight, 0, GameConstants.cellWidth / 2), 90, outer);
                drawnWalls.Add(positionX + "," + positionY + ",2");
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
                positionX = positionX - 1;
                positionY = positionY - 1;
            }
        }

        // Verificar y agregar paredes y puertas derecha
        if (!drawnWalls.Contains(positionX + "," + (positionY + 1) + ",1") && !drawnWalls.Contains(positionX + "," + positionY + ",3"))
        {
            if (tile.getWall().getRight() == 1)
            {
                outer = (positionY == GameConstants.cols - 1) ? true : false;
                CreateWall(cellPosition + new Vector3(GameConstants.cellHeight / 2, 0, GameConstants.cellWidth + GameConstants.wallThickness), 0, outer);
                drawnWalls.Add(positionX + "," + positionY + ",3");
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
                positionX = positionX - 1;
                positionY = positionY - 1;
            }
        }

        CheckCorners(tile, cellPosition);
    }

    void Update()
    {
        if (destroyList.Count == 0)
        {
            Debug.Log("No hay POIs para eliminar.");
            return;
        }
        else
        {
            foreach (GameObject obj in destroyList)
            {
                Debug.Log("Eliminando POI en: " + obj.transform.position);
                Debug.Log("Name: " + obj.name);
                if (obj is GameObject)
                {
                    Debug.Log("Es un GameObject.");
                    obj.SetActive(false);
                    obj.transform.position = new Vector3(1000, 1000, 1000);
                }
                Destroy(obj);
            }
            destroyList.Clear();
        }
        

    }

    void HandleWall(int wallHealth, Vector3 wallPosition)
    {
        if (wallDict.TryGetValue(wallPosition, out GameObject wallObject))
        {
            if (wallHealth == 0)
            {
                Destroy(wallObject);
                wallDict.Remove(wallPosition);
                Debug.LogWarning("Pared destruida en la posición: " + wallPosition);
            }
            else if (wallHealth == 2)
            {
                Renderer wallRenderer = wallObject.GetComponent<Renderer>();
                if (wallRenderer != null)
                {
                    wallRenderer.material = GameConstants.damagedMaterial;
                    Debug.Log("Material de la pared cambiado a damagedMaterial en: " + wallPosition);
                }
                else
                {
                    Debug.LogWarning("El objeto no tiene un componente Renderer.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No se encontró ninguna pared en la posición: " + wallPosition);
        }
    }

    void HandleDoor(int doorHealth, Vector3 doorPosition)
    {
        if (doorDict.TryGetValue(doorPosition, out GameObject doorObject))
        {
            if (doorHealth == 0)
            {
                Destroy(doorObject);
                doorDict.Remove(doorPosition);
                Debug.LogWarning("Puerta destruida en la posición: " + doorPosition);
            }
        }
        else
        {
            Debug.LogWarning("No se encontró ninguna puerta en la posición: " + doorPosition);
        }
    }

    void ReplaceWithOpenedDoor(Vector3 doorPosition, float rotationY)
    {
        if (doorDict.TryGetValue(doorPosition, out GameObject doorObject))
        {

            Destroy(doorObject);
            doorDict.Remove(doorPosition);

            GameObject doorOpened = Instantiate(doorOpenedPrefab, doorPosition, Quaternion.Euler(0, rotationY, 0));
        }
    }

    public void PlaceWalls()
    {
        foreach (KeyValuePair<Vector2Int, Tile> kvp in tileDict)
        {
            UpdateEnvironment(kvp);
        }
    }


    // Nuevo método para actualizar individualmente cada Tile
    public void UpdateTile(AffectedTilesData tileData)
    {
        // Buscamos el Tile correspondiente según las coordenadas
        //vTile tileToUpdate = FindTile(tileData.x, tileData.y);
        Vector2Int position = new Vector2Int(tileData.x, tileData.y);
        Tile tileToUpdate = tileDict[position];

        if (tileToUpdate != null)
        {
            // Actualizamos el Tile con los nuevos datos
            tileToUpdate.UpdateTile(tileData);
            tileDict[position] = tileToUpdate;
            UpdateEnvironment(new KeyValuePair<Vector2Int, Tile>(position, tileToUpdate));

            Debug.Log($"Tile at ({tileData.x}, {tileData.y}) updated successfully.");
        }
        else
        {
            Debug.LogWarning($"Tile at ({tileData.x}, {tileData.y}) not found.");
        }
        
    }

    // Método para encontrar un Tile en la lista basada en sus coordenadas
    private Tile FindTile(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);
        if (tileDict.TryGetValue(position, out Tile tile))
        {
            return tile;
        }
        else
        {
            return null;
        }
    }

    public void UpdateDictionary(Tile tile, Vector2Int position)
    {
        // Update the tile of the dictionary
        tileDict[position] = tile;
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
        // Almacena la puerta en el diccionario
        if (!doorDict.ContainsKey(position))
        {
            doorDict.Add(position, doorWall);
        }
    }

    void CreateWall(Vector3 position, float rotationY, bool outer) // Crear pared
    {
        GameObject wall = null;
        if (outer) // Si la pared es exterior...
        {
            // Generar un n�mero aleatorio para decidir qu� objeto instanciar
            int randomIndex = UnityEngine.Random.Range(0, 4);

            // Instanciar la pared si el numero es diferente a 0
             wall = (randomIndex != 0) ? Instantiate(wallPrefab, position, Quaternion.Euler(0, rotationY, 0)) :
                                    Instantiate(wallWindowPrefab, position, Quaternion.Euler(0, rotationY, 0));
        }
        else // Si la pared no es exterior...
        {
            wall = Instantiate(wallPrefab, position, Quaternion.Euler(0, rotationY, 0));
        }

        // Almacena la pared en el diccionario
        if (!wallDict.ContainsKey(position))
        {
            wallDict.Add(position, wall);
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

            while (poiTransform != null)
            {
                // Calculate the new Y position using a sine wave
                float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
                // Update the POI's position
                poiTransform.position = new Vector3(poiTransform.position.x, newY, poiTransform.position.z);
                yield return null;
            }
            yield break;
        }
    }
}
