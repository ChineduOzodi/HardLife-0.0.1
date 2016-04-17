﻿using UnityEngine;
using System.Collections;
using System;

public class FresNoise
{
    //public int pixWidth;
    //public int pixHeight;
    //public float xOrg;
    //public float yOrg;
    //public float scale = 1.0F;

    public float[,] CalcNoise(int pixWidth, int pixHeight, float xOrg, float yOrg, float scale = 1f)
    {
        float[,] map = new float[pixWidth, pixHeight];
        int y = 0;
        while (y < pixHeight)
        {
            int x = 0;
            while (x < pixWidth)
            {
                float xCoord = xOrg + x / pixWidth * scale;
                float yCoord = yOrg + y / pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                map[x, y] = sample;
                x++;
            }
            y++;
        }

        return map;
    }

    public float[,] CalcNoise(int pixWidth, int pixHeight, string seed = null, float scale = 1f)
    {
        if (seed == null)
        {
            seed = Time.time.ToString();
        }
        System.Random randNum = new System.Random(seed.GetHashCode());

        float xOrg = randNum.Next();
        float yOrg = xOrg;

        float[,] map = new float[pixWidth, pixHeight];
        int y = 0;
        while (y < pixHeight)
        {
            int x = 0;
            while (x < pixWidth)
            {
                float xCoord = xOrg + x / pixWidth * scale;
                float yCoord = yOrg + y / pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                map[x, y] = sample;
                x++;
            }
            y++;
        }

        return map;
    }

    public int[,] CalcNoise(int pixWidth, int pixHeight,float[] heightMap, string seed = null, float scale = 1f)
    {
        if (seed == null)
        {
            seed = Time.time.ToString();
        }
        System.Random randNum = new System.Random(seed.GetHashCode());

        
        float xOrg = randNum.Next();
        float yOrg = xOrg;

        int[,] map = new int[pixWidth, pixHeight];
        int y = 0;
        while (y < pixHeight)
        {
            int x = 0;
            while (x < pixWidth)
            {
                float xCoord = xOrg + x / pixWidth * scale;
                float yCoord = yOrg + y / pixHeight * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                map[x, y] = ScaleFloatToInt(sample,heightMap);
                x++;
            }
            y++;
        }

        return map;

    }

    public int ScaleFloatToInt(float sample, float[] heightMap)
    {
        int num = 0;

        for (int i = 0; i < heightMap.Length; i++)
        {
            if (heightMap[i] < sample)
            {
                num = i;
                break;
            }
        }

        return num;
    }

}