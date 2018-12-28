package com.example.avbal.myapplication.Sensors;

public class SensorData
{
    public final float[] values = new float[6];
    private final long timestamp;

    public SensorData(float[] data, int size, long timestamp)
    {
        System.arraycopy(data, 0, values, 0, size);
        this.timestamp = timestamp;
    }

    public String toString()
    {
        return String.format("SD;%f;%f;%f;%d\r\n", values[0], values[1], values[2], timestamp);
    }
}
