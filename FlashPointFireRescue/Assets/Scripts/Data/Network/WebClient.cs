// TC2008B Modelación de Sistemas Multiagentes con gráficas computacionales
// C# client to interact with Python server via POST
// Sergio Ruiz-Loza, Ph.D. March 2021

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class WebClient : MonoBehaviour
{
    // IEnumerator - yield return
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
            //www.SetRequestHeader("Content-Type", "text/html");
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();          // Talk to Python
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
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
                // wallAndDoor.ReceiveData(result);
                //Vector3 tPos = JsonUtility.FromJson<Vector3>(www.downloadHandler.text.Replace('\'', '\"'));


                /*
                string jsonResponse = www.downloadHandler.text;

                Debug.Log("JsonResponse" + jsonResponse); 

                // Dividir el string por la palabra "data"
                string[] initial = jsonResponse.Substring(1, jsonResponse.Length - 2).Split(new string[] { "type" }, StringSplitOptions.None);

                // Mostrar las partes divididas
                foreach (string part in initial)
                {
                    Debug.Log(part);
                }

                FixString(initial[1]);
                
                Debug.Log(www.downloadHandler.text);    // Answer from Python
                string tPos = www.downloadHandler.text;
                
                WallAndDoorPlacement wallAndDoor = FindObjectOfType<WallAndDoorPlacement>();
                wallAndDoor.ReceiveData(tPos);
                //Vector3 tPos = JsonUtility.FromJson<Vector3>(www.downloadHandler.text.Replace('\'', '\"'));
                Debug.Log("Form upload complete!");
                */
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        //string call = "What's up?";
        Vector3 fakePos = new Vector3(3.44f, 0, -15.707f);
        string json = EditorJsonUtility.ToJson(fakePos);
        //StartCoroutine(SendData(call));
        StartCoroutine(SendData(json));
        // transform.localPosition
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixString(string str)
    {
        int startIndex = str.IndexOf('{');
        int endIndex = str.LastIndexOf('}');

        // Obtiene el substring desde el primer '{' hasta el último '}'
        string result = str.Substring(startIndex, endIndex - startIndex + 1);
        WallAndDoorPlacement wallAndDoor = FindObjectOfType<WallAndDoorPlacement>();
        wallAndDoor.ReceiveData(result);
    }
}