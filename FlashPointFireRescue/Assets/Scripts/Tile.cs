using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WebClient;

public class Tile
{
    private Wall wall;
    private int fireStatus;
    private bool hasPOI;
    private int numberVictims;
    List<int> fireFighters;
    
    public Tile(
        int top, int left, int bottom, int right, bool isOpen, 
        int topHealth, int leftHealth, int bottomHealth, int rightHealth, 
        int fireStatus, bool hasPOI, int numberVictims, List<int> fireFighters)
    {
        this.wall = new Wall(top, left, bottom, right, isOpen, topHealth, leftHealth, bottomHealth, rightHealth);
        this.fireStatus = fireStatus;
        this.hasPOI = hasPOI;
        this.numberVictims = numberVictims;
        this.fireFighters = fireFighters;
    }


    // Métodos 'get'
    public Wall getWall() => this.wall;
    public int getFireStatus() => this.fireStatus;
    public bool getHasPOI() => this.hasPOI;
    public int getNumberVictims() => this.numberVictims;
    public List<int> getFireFighters() => this.fireFighters;


    // Métodos 'set'
    public void setFireStatus(int fireStatus) => this.fireStatus = fireStatus;
    public void setHasPOI(bool hasPOI) => this.hasPOI = hasPOI;
    public void setNumberVictims(int numberVictims) => this.numberVictims = numberVictims;
    public void setFireFighters(List<int> fireFighters) => this.fireFighters = fireFighters;


    // Método para actualizar el Tile usando AffectedTilesData
    public void UpdateTile(AffectedTilesData data)
    {
        this.wall.UpdateWall(data.top, data.left, data.bottom, data.right, data.isOpen, data.topHealth, data.leftHealth, data.bottomHealth, data.rightHealth);
        this.setFireStatus(data.fireStatus);
        this.setHasPOI(data.hasPOI);
        this.setNumberVictims(data.numberOfVictims);
        this.setFireFighters(new List<int>(data.firefightersIDs));
    }

}
