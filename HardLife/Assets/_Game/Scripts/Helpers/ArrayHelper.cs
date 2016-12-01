using UnityEngine;
using System.Collections;

public struct ArrayHelper {

    /// <summary>
    /// Element Index of a 2d coord in a 1D array
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="width">Width of the 2d coords</param>
    /// <returns></returns>
    public static int ElementIndex(int row, int col, int width)
    {
        return width * row + col;
    }
    /// <summary>
    /// Converts 2D array to 1D array
    /// </summary>
    /// <param name="array2D"></param>
    /// <returns></returns>
    public static int[] TwoDToOneD(int[,] array2D)
    {
        int width = array2D.GetLength(0);
        int height = array2D.GetLength(1);
        int[] array = new int[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                array[ElementIndex(x, y, width)] = array2D[x, y];
            }
        }

        return array;
   }

    public static float[] TwoDToOneD(float[,] array2D)
    {
        int width = array2D.GetLength(0);
        int height = array2D.GetLength(1);
        float[] array = new float[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                array[ElementIndex(x, y, width)] = array2D[x, y];
            }
        }

        return array;
    }
}
