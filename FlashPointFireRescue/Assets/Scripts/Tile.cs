using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Wall getWall()
    {
        return this.wall;
    }
    
    public int getFireStatus()
    {
        return this.fireStatus;
    }

    public bool getHasPOI()
    {
        return this.hasPOI;
    }

    public int getNumberVictims()
    {
        return this.numberVictims;
    }

    public List<int> getFireFighters()
    {
        return this.fireFighters;
    }
    
    public void setFireStatus(int fireStatus)
    {
        this.fireStatus = fireStatus;
    }

    public void setHasPOI(bool hasPOI)
    {
        this.hasPOI = hasPOI;
    }

    public void setNumberVictims(int numberVictims)
    {
        this.numberVictims = numberVictims;
    }

    public void setFireFighters(List<int> fireFighters)
    {
        this.fireFighters = fireFighters;
    }
    
}
