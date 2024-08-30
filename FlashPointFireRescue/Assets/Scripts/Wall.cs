using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    private int top;
    private int left;
    private int bottom;
    private int right;
    private int isDoor;
    private bool isOpen;

    public Wall(int top, int left, int bottom, int right, int isDoor, bool isOpen=false)
    {
        this.top = top;
        this.left = left;
        this.bottom = bottom;
        this.right = right;
        this.isDoor = isDoor;
        this.isOpen = isOpen;

        if (isDoor == 1)
        {
            this.top = 2;
        }
        else if (isDoor == 2)
        {
            this.left = 2;
        }
        else if (isDoor == 3)
        {
            this.bottom = 2;
        }
        else if (isDoor == 4)
        {
            this.right = 2;
        }
    }
    
    public int getTop()
    {
        return this.top;
    }

    public int getLeft()
    {
        return this.left;
    }

    public int getBottom()
    {
        return this.bottom;
    }

    public int getRight()
    {
        return this.right;
    }

    public int getIsDoor()
    {
        return this.isDoor;
    }

    public bool getIsOpen()
    {
        return this.isOpen;
    }

    public void setTop(int top)
    {
        this.top = top;
    }

    public void setLeft(int left)
    {
        this.left = left;
    }

    public void setBottom(int bottom)
    {
        this.bottom = bottom;
    }

    public void setRight(int right)
    {
        this.right = right;
    }

    public void setIsDoor(int isDoor)
    {
        this.isDoor = isDoor;
    }

    public void setIsOpen(bool isOpen)
    {
        this.isOpen = isOpen;
    }
}
