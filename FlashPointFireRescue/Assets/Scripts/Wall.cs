using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    private int top;
    private int left;
    private int bottom;
    private int right;
    private bool isOpen;
    private int topHealth;
    private int leftHealth;
    private int bottomHealth;
    private int rightHealth;

    public Wall(int top, int left, int bottom, int right, bool isOpen, int topHealth, int leftHealth, int bottomHealth, int rightHealth)
    {
        this.top = top;
        this.left = left;
        this.bottom = bottom;
        this.right = right;
        this.isOpen = isOpen;
        this.topHealth = topHealth;
        this.leftHealth = leftHealth;
        this.bottomHealth = bottomHealth;
        this.rightHealth = rightHealth;
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

    public bool getIsOpen()
    {
        return this.isOpen;
    }

    public int getTopHealth()
    {
        return this.topHealth;
    }

    public int getLeftHealth()
    {
        return this.leftHealth;
    }

    public int getBottomHealth()
    {
        return this.bottomHealth;
    }

    public int getRightHealth()
    {
        return this.rightHealth;
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

    public void setIsOpen(bool isOpen)
    {
        this.isOpen = isOpen;
    }

    public void setTopHealth(int topHealth)
    {
        this.topHealth = topHealth;
    }

    public void setLeftHealth(int leftHealth)
    {
        this.leftHealth = leftHealth;
    }

    public void setBottomHealth(int bottomHealth)
    {
        this.bottomHealth = bottomHealth;
    }

    public void setRightHealth(int rightHealth)
    {
        this.rightHealth = rightHealth;
    }
}
