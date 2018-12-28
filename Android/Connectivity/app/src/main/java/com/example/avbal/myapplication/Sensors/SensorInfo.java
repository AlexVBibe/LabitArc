package com.example.avbal.myapplication.Sensors;

public class SensorInfo
{
    private final String name;
    private final int type;

    public SensorInfo(String name, int type)
    {
        this.name = name;
        this.type = type;
    }

    public String getName()
    {
        return name;
    }

    public int getType()
    {
        return type;
    }
}
