using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class POIManager : MonoBehaviour
{

    private string poiStrInput = @"
    2 4 v
    5 1 f
    5 8 v
    ";

    private string[] poiStrArr;
    public GameObject questionPrefab;
    [SerializeField] private Vector3 poiRotation = Vector3.zero;

    // private int POIS = 18;
    private int activePOIS = 0;
    // private int POIFalse = 6;
    // private int POIVictims = 12;

    private float floatSpeed = 1.5f;
    private float floatAmplitude = 1.8f;

    void CreatePOI(int file, int column, string poiType)
    {
        float x = 7f + ((file - 1f) * (10f));
        float y = 2f;
        float z = 5f + ((column - 1f) * 10f);

        Vector3 position = new Vector3(x, y, z);
        GameObject poi = Instantiate(questionPrefab, position, Quaternion.Euler(poiRotation));

        if (poiType == "v")
        {
            poi.name = "Victim POI";
        }
        else if (poiType == "f")
        {
            poi.name = "False POI";
        }

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

    // Start is called before the first frame update
    void Start()
    {

        poiStrArr = poiStrInput.Trim().Split('\n');

        foreach (string line in poiStrArr)
        {
            string[] poiParts = line.Trim().Split(' ');

            // Initialize the 3 given POI at the start of the game at the given locations
            if (poiParts.Length == 3)
            {
                int file = int.Parse(poiParts[0]);
                int column = int.Parse(poiParts[1]);
                string poiType = poiParts[2];

                CreatePOI(file, column, poiType);
                activePOIS++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
