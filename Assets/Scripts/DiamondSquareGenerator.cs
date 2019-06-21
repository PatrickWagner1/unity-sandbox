using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the diamond square algorithm.
/// </summary>
public class DiamondSquareGenerator
{

    /// <summary>
    /// Calculates the total side length of the mesh.
    /// </summary>
    /// <returns>The total side length</returns>
    public static int getTotalSize(int size)
    {
        return (int)Mathf.Pow(2, size) + 1;
    }

    /// <summary>
    /// Returns a map of heights calculated with the diamond square algorithm.
    /// </summary>
    /// <returns>Map of heights</returns>
    public static float[,] diamondSquare(int size, float rough, int seed)
    {
        Random.InitState(seed);
        int totalSize = getTotalSize(size);
        int depth = totalSize - 1;
        float[,] map = new float[totalSize, totalSize];
        map[0, 0] = Random.value;
        map[0, depth] = Random.value;
        map[depth, 0] = Random.value;
        map[depth, depth] = Random.value;

        float average;
        float range = seed;
        int halfSide;

        for (int sideLength = totalSize - 1; sideLength > 1; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            // Diamond step
            for (int x = 0; x < depth; x += sideLength)
            {
                for (int z = 0; z < depth; z += sideLength)
                {
                    average = (map[x, z] + map[x + sideLength, z] +
                            map[x, z + sideLength] + map[x + sideLength, z + sideLength]) / 4.0f +
                        (Random.value * (range * 2.0f)) - range;
                    map[x + halfSide, z + halfSide] = average;
                }
            }

            // Square step
            for (int x = 0; x < depth; x += halfSide)
            {
                for (int z = (x + halfSide) % sideLength; z < depth; z += sideLength)
                {
                    average = (map[(x - halfSide + depth) % depth, z] +
                            map[(x + halfSide) % depth, z] +
                            map[x, (z + halfSide) % depth] +
                            map[x, (z - halfSide + depth) % depth]) / 4.0f +
                        (Random.value * (range * 2.0f)) - range;

                    map[x, z] = average;

                    if (x == 0)
                    {
                        map[depth, z] = average;
                    }

                    if (z == 0)
                    {
                        map[x, depth] = average;
                    }
                }
            }

            range -= range * 0.5f * rough;
        }

        return map;
    }
}