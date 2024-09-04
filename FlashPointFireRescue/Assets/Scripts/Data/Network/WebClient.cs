using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WebClient : MonoBehaviour
{
    IEnumerator SendData(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("bundle", "the data");
        string url = "http://localhost:8585";
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();          // Talk to Python
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                Dictionary<string, Tile> dictString = JsonConvert.DeserializeObject<Dictionary<string, Tile>>(jsonResponse);
                Dictionary<Vector2Int, Tile> houseFire = new Dictionary<Vector2Int, Tile>();
                foreach (KeyValuePair<string, Tile> entry in dictString)
                {
                    string[] key = entry.Key.Split(',');
                    Vector2Int gridPosition = new Vector2Int(int.Parse(key[0]), int.Parse(key[1]));
                    Tile tile = entry.Value;
                    houseFire.Add(gridPosition, tile);
                }

                foreach (KeyValuePair<Vector2Int, Tile> entry in houseFire)
                {
                    Vector2Int gridPosition = entry.Key;
                    Tile tile = entry.Value;
                    Debug.Log("Key: " + gridPosition + " Value: " + tile);
                }

                WallAndDoorPlacement wallAndDoor = FindObjectOfType<WallAndDoorPlacement>();
                wallAndDoor.setDictionary(houseFire);
                wallAndDoor.PlaceWalls();

                Debug.Log("Form upload complete!");

                StartCoroutine(SendSecondRequest());
            }
        }
    }

    IEnumerator SendSecondRequest()
    {
        string url = "http://localhost:8585";

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string secondResponseData = www.downloadHandler.text;
                Debug.Log("Second request response: " + secondResponseData);

                // Inicia la corrutina para procesar los datos
                StartCoroutine(ProcessResponseData(secondResponseData));
            }
        }
    }

    IEnumerator ProcessResponseData(string secondResponseData)
    {
        var responseDict = JsonConvert.DeserializeObject<Dictionary<string, List<BotRunData>>>(secondResponseData);


        int runCounter = 0;
        foreach (var run in responseDict.Values)
        {
            Debug.Log($"Current_run: {runCounter}");
            runCounter++;
            foreach (var botData in run)
            {
                Debug.Log($"Current firefighter id: {botData.bot_id}");
                foreach (var stepData in botData.agent_step_data)
                {
                    Debug.Log($"Current agent step: {stepData.model_step_id}");
                    var tileData = stepData.affected_tiles_data;
                    UpdateTile(tileData);
                    Debug.Log($"Tile data damageCounter: {tileData.damageCounter}, Tile data savedVictims: {tileData.savedVictims},  Tile data deadVictims: {tileData.deadVictims}");
                    yield return new WaitForSeconds(0.125f);


                    CoinGridManager coinGridManager = FindObjectOfType<CoinGridManager>();
                    if (coinGridManager != null)
                    {
                        coinGridManager.UpdateGridElements(tileData.savedVictims, tileData.deadVictims);
                    }
                    GridElementManager gridElementManager = FindObjectOfType<GridElementManager>();
                    if (gridElementManager != null)
                    {
                        gridElementManager.UpdateGridElements(tileData.damageCounter);
                    }
                    


                    if (tileData.savedVictims == 7)
                    {
                        GameManager gameManager = FindObjectOfType<GameManager>();
                        string end = "Oro robado!";
                        gameManager.EndGame(end);
                        break;
                    }
                    else if(tileData.deadVictims == 4)
                    {
                        GameManager gameManager = FindObjectOfType<GameManager>();
                        string end = "4 bolsas de oro perdidas...";
                        gameManager.EndGame(end);
                        break;
                    }
                    else if (tileData.damageCounter == 24)
                    {
                        GameManager gameManager = FindObjectOfType<GameManager>();
                        string end = "Casa demolida";
                        gameManager.EndGame(end);
                        break;
                    }
                }
            }
        }
    }

    void UpdateTile(AffectedTilesData tileData)
    {
        // Aquí actualizas las propiedades del Tile basado en tileData
        Debug.Log($"Updating Tile at ({tileData.x}, {tileData.y}) with new data.");
        WallAndDoorPlacement wallAndDoor = FindObjectOfType<WallAndDoorPlacement>();
        wallAndDoor.UpdateTile(tileData);


    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Sending first request...");
        Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
        #if UNITY_EDITOR
        string json = EditorJsonUtility.ToJson(fakePos);
        #else
        string json = JsonUtility.ToJson(fakePos);
        #endif
        StartCoroutine(SendData(json));
        Debug.Log("First request sent. Waiting to send second request...");
    }

    // Update is called once per frame
    void Update()
    {
    }


    public class BotRunData
    {
        public int bot_id { get; set; }
        public List<AgentStepData> agent_step_data { get; set; }
    }

    public class AgentStepData
    {
        public int model_step_id { get; set; }
        public AffectedTilesData affected_tiles_data { get; set; }
    }

    public class AffectedTilesData
    {
        public int x { get; set; }
        public int y { get; set; }
        public int top { get; set; }
        public int left { get; set; }
        public int bottom { get; set; }
        public int right { get; set; }
        public bool isOpen { get; set; }
        public int topHealth { get; set; }
        public int leftHealth { get; set; }
        public int bottomHealth { get; set; }
        public int rightHealth { get; set; }
        public int fireStatus { get; set; }
        public bool hasPOI { get; set; }
        public int numberOfVictims { get; set; }
        public List<int> firefightersIDs { get; set; }
        public string actions { get; set; }
        public int dx { get; set; }
        public int dy { get; set; }
        public int damageCounter { get; set; }
        public string poi { get; set; }
        public int savedVictims { get; set; }
        public int deadVictims { get; set; }
    }



    
}