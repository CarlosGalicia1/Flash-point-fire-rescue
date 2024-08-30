using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private Wall wall;
    private int fireStatus;
    private bool hasPOI;
    private bool isVictime;
    
    public Tile(int top, int left, int bottom, int right, int isDoor, bool isOpen=false)
    {
        this.wall = new Wall(top, left, bottom, right, isDoor, isOpen);
        this.fireStatus = 0;
        this.hasPOI = false;
        this.isVictime = false;
    }

    public int getFireStatus()
    {
        return this.fireStatus;
    }

    public bool getHasPOI()
    {
        return this.hasPOI;
    }

    public bool getIsVictime()
    {
        return this.isVictime;
    }
    
    public void setFireStatus(int fireStatus)
    {
        this.fireStatus = fireStatus;
    }

    public void setHasPOI(bool hasPOI)
    {
        this.hasPOI = hasPOI;
    }

    public void setIsVictime(bool isVictime)
    {
        this.isVictime = isVictime;
    }

    
}
