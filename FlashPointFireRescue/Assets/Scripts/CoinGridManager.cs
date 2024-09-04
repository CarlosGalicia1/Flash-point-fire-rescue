using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CoinGridManager : MonoBehaviour
{
    public Image[] gridElements; // Arreglo para almacenar las imágenes GridElement
    void Start()
    {
        // Inicializa el arreglo con los elementos GridElement
        gridElements = GetComponentsInChildren<Image>();
    }

    public void UpdateGridElements(int savedVictims, int deadVictims)
    {
        // Recorre las primeras 'savedVictims' imágenes y cambia su color a blanco
        for (int i = 0; i < savedVictims && i < gridElements.Length; i++)
        {
            gridElements[i +1].color = GameConstants.whiteColor;
        }

        // Recorre las primeras 'deadVictims' imágenes y cambia su color a rojo
        for (int i = 0; i < deadVictims && i < gridElements.Length; i++)
        {
            gridElements[gridElements.Length - 1 - i].color = GameConstants.redColor;
        }
    }
}
