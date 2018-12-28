package com.example.avbal.myapplication.Sensors;

public class SensorData
{
    private final float[] eventData = new float[6];
    private final long timestamp;

    public SensorData(float[] data, int size, long timestamp)
    {
        System.arraycopy(data, 0, eventData, 0, size);
        this.timestamp = timestamp;
    }

    public String toString()
    {
        String string = eventData.toString();
        return String.format("SD;%f;%f;%f;%d\r\n", eventData[0], eventData[1], eventData[2], timestamp);
    }
}
