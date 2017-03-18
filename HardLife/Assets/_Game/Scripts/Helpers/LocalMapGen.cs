using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CodeControl;

public struct LocalMapGen {

    private static float[] baseMapNC = { .5f, .55f, .85f, .9f };
    private static float[] itemMapNC = { .33f, .66f };

    public static void GenerateLocalMap(LocalMapModel model)
    {

        bool hasShores = false;

        WorldModel world = model.world.Model;

        world.localMapWorldBottomLeft = -Vector3.right * world.localSize.x / 2 - Vector3.up * world.localSize.y / 2;
        model.worldBottomLeft = world.localMapWorldBottomLeft;

        // Setup Helper Maps elevation map
        FresNoise noise = new FresNoise();
        float[,] heightMap = noise.CalcNoise(model.localSizeX, model.localSizeY, model.seed, model.heightMapScale);
        float[,] hMap = heightMap;
        float[,] hHelperMap = GenerateHorHelperMap(model.localSizeX, model.localSizeY);
        float[,] vHelperMap = GenerateVertHelperMap(model.localSizeX, model.localSizeY);
        float[,] cHelperMap = GenerateCorHelperMap(model.localSizeX, model.localSizeY, hHelperMap, vHelperMap);

        LocalMapModel[,] adjacentLocalMaps = AdjacentLocalMaps(model);

        //Check if has shores
        foreach (LocalMapModel LocalMap in adjacentLocalMaps)
        {
            if (LocalMap.biome == "Water")
            {
                hasShores = true;
                break;
            }
        }

        #region "Heigh Map Generation"
        for (int x = 0; x < model.localSizeX; x++)
        {
            for (int y = 0; y < model.localSizeY; y++)
            {
                if (x <= model.localSizeX / 2 && y <= model.localSizeY / 2) //Bottom Left Generation
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[0, 1].elevation - model.elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 0].elevation - model.elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[0, 0].elevation - model.elevation);

                    if (adjacentLocalMaps[0, 1].biome == "Water") //Water application
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[0, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * model.baseMapScale;
                    }
                }
                else if (x <= model.localSizeX / 2 && y > model.localSizeY / 2)
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[0, 1].elevation - model.elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 2].elevation - model.elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[0, 2].elevation - model.elevation);

                    if (adjacentLocalMaps[0, 1].biome == "Water") //Water Application
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[0, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * model.baseMapScale;
                    }
                }
                else if (x > model.localSizeX / 2 && y <= model.localSizeY / 2)
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[2, 1].elevation - model.elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 0].elevation - model.elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[2, 0].elevation - model.elevation);

                    if (adjacentLocalMaps[2, 1].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[2, 0].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * model.baseMapScale;
                    }
                }
                else if (x > model.localSizeX / 2 && y > model.localSizeY / 2)
                {
                    //Adjacent Elevation Application
                    heightMap[x, y] += (1 - vHelperMap[x, y]) * (adjacentLocalMaps[2, 1].elevation - model.elevation);
                    heightMap[x, y] += (1 - hHelperMap[x, y]) * (adjacentLocalMaps[1, 2].elevation - model.elevation);
                    heightMap[x, y] += (1 - cHelperMap[x, y]) * (adjacentLocalMaps[2, 2].elevation - model.elevation);

                    if (adjacentLocalMaps[2, 1].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - vHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[1, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - hHelperMap[x, y]) * model.baseMapScale;
                    }
                    if (adjacentLocalMaps[2, 2].biome == "Water")
                    {
                        heightMap[x, y] -= (1 - cHelperMap[x, y]) * model.baseMapScale;
                    }
                }
            }
        }
        #endregion
        heightMap = MapScaler(model.localSizeX, model.localSizeY, heightMap);

        model.elevationMap = ArrayHelper.TwoDToOneD(heightMap); //Set local elevation map

        CreateBaseMap(model, noise, hasShores);
        CreateObjectMap(model);
    }

    

    private static void CreateObjectMap(LocalMapModel model)
    {
        model.objectMap = new BaseObjectModel[model.localSizeX * model.localSizeY];
        System.Random random = new System.Random(model.seed.GetHashCode());

        double treeLikeliness = .02; //Probaility of loading items
        double bushLikeliness = .02;
        double boulderLikeliness = .01;

        if (model.biome == "Jungle")
        {
            treeLikeliness = .03;
            bushLikeliness = .01;
        }

        for (int x = 0; x < model.localSizeX; x++)
        {
            for (int y = 0; y < model.localSizeY; y++)
            {
                Vector3 worldPoint = model.worldBottomLeft + Vector3.right * (x + .5f) + Vector3.up * (y + .5f);
                int index = ArrayHelper.ElementIndex(x, y, model.localSizeY);

                if (model.biome != "Water") //water local map
                {
                    if (model.baseMap[index].name == "Grass")
                    {
                        if (random.NextDouble() < treeLikeliness)
                        {
                            TreeModel treeModel = new TreeModel();
                            treeModel.localMap = new ModelRef<LocalMapModel>(model);
                            CreateObjectModel.SetTreeModel(treeModel, "oak tree", new Date(-random.Next(100) * Date.Year), worldPoint, x,y);
                            model.objectMap[index] = treeModel;
                        }
                        else if (random.NextDouble() < bushLikeliness)
                        {
                            TreeModel treeModel = new TreeModel();
                            treeModel.localMap = new ModelRef<LocalMapModel>(model);
                            CreateObjectModel.SetTreeModel(treeModel, "bush", new Date(-random.Next(24) * Date.Year), worldPoint, x, y);
                            model.objectMap[index] = treeModel;
                        }
                        else if (random.NextDouble() < boulderLikeliness)
                        {
                            BaseObjectModel otherModel = new BaseObjectModel();
                            otherModel.localMap = new ModelRef<LocalMapModel>(model);
                            CreateObjectModel.SetBaseObjectModel(otherModel, "boulder", worldPoint, x, y);
                            otherModel.walkSpeedMod = .2f;
                            model.objectMap[index] = otherModel;

                        }
                        else if (model.baseMap[index].name == "Rock")
                        {
                            BaseObjectModel otherModel = new BaseObjectModel();
                            otherModel.localMap = new ModelRef<LocalMapModel>(model);
                            otherModel.walkSpeedMod = 0;
                            CreateObjectModel.SetBaseObjectModel(otherModel, "rock", worldPoint, x, y);
                            model.objectMap[index] = otherModel;

                        }
                    }
                }

            }


        }
    }

    private static void CreateBaseMap(LocalMapModel model, FresNoise noise, bool hasShores)
    {
        model.baseMap = new TileModel[model.localSizeX * model.localSizeY];

        for (int x = 0; x < model.localSizeX; x++)
        {
            for (int y = 0; y < model.localSizeY; y++)
            {
                Vector3 worldPoint = model.worldBottomLeft + Vector3.right * (x + .5f) + Vector3.up * (y + .5f);
                int index = ArrayHelper.ElementIndex(x, y, model.localSizeY);
                model.baseMap[index] = CreateObjectModel.CreateTileModel(worldPoint, x, y, noise.ScaleFloatToInt(model.elevationMap[index], baseMapNC));


                if (model.biome == "Water") //water local map
                {
                    model.baseMap[index].name = "Water";
                }
                else if (model.baseMap[index].id >= 4)
                {
                    model.baseMap[index].name = "Rock";
                }
                else if (model.biome == "Ice")
                {

                    model.baseMap[index].name = "Ice";
                }
                else if (model.biome == "Grass")
                {

                    model.baseMap[index].name = "Grass";
                }
                else if (model.biome == "Jungle")
                {

                    model.baseMap[index].name = "Grass";
                }
                else if (model.biome == "Desert")
                {

                    model.baseMap[index].name = "Sand";
                }

                if (hasShores && model.baseMap[index].id == 0)
                {
                    model.baseMap[index].name = "Water";
                }
                else if (hasShores && model.baseMap[index].id == 1 && model.biome != "Ice" && model.biome != "Water" && model.biome != "Desert")
                {
                    model.baseMap[index].name = "Sand";
                }

            }


        }
    }

    private static LocalMapModel[,] AdjacentLocalMaps(LocalMapModel model)
    {
        LocalMapModel[,] adj = new LocalMapModel[3, 3];

        for (int nbrX = model.worldMapPositionX - 1; nbrX <= model.worldMapPositionX + 1; nbrX++)
        {
            for (int nbrY = model.worldMapPositionY - 1; nbrY <= model.worldMapPositionY + 1; nbrY++)
            {
                if (IsInWorldMapRange(model.world.Model, nbrX, nbrY))
                {
                    adj[nbrX + 1 - model.worldMapPositionX, nbrY + 1 - model.worldMapPositionY] = model.world.Model.localMaps[ArrayHelper.ElementIndex(nbrX, nbrY,model.worldMapPositionY)];
                }
                else
                {
                    adj[nbrX + 1 - model.worldMapPositionX, nbrY + 1 - model.worldMapPositionY] = null;
                }

            }
        }

        return adj;
    }

    public static BaseObjectModel[,] AdjacentObjects(LocalMapModel model, int x, int y)
    {
        BaseObjectModel[,] adj = new BaseObjectModel[3, 3];

        for (int nbrX = x - 1; nbrX <= x + 1; nbrX++)
        {
            for (int nbrY = y - 1; nbrY <= y + 1; nbrY++)
            {
                if (IsInLocalMapRange(model, nbrX, nbrY))
                {
                    if (model.objectMap[ArrayHelper.ElementIndex(nbrX, nbrY,model.localSizeY)] != null)
                    {
                        adj[nbrX + 1 - x, nbrY + 1 - y] = model.objectMap[ArrayHelper.ElementIndex(nbrX, nbrY,model.localSizeY)];
                    }
                    
                }
            }
        }

        return adj;
    }
    private static bool IsInWorldMapRange(WorldModel world, int x, int y)
    {
        if (x >= 0 && x < world.worldSizeX && y >= 0 && y < world.worldSizeY)
            return true;
        else
            return false;
    }
    public static bool IsInLocalMapRange(LocalMapModel model, int x, int y)
    {
        if (x >= 0 && x < model.localSizeX && y >= 0 && y < model.localSizeX)
            return true;
        else
            return false;
    }
    private static float[,] MapScaler(int localSizeX, int localSizeY, float[,] map) //Scales float Maps Betwee 0 and 1
    {
        float max = map[0, 0];
        float min = map[0, 0];
        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                if (map[x, y] > max)
                    max = map[x, y];
                if (map[x, y] < min)
                    min = map[x, y];
            }
        }

        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                map[x, y] = (map[x, y] - min) / (max - min);
            }
        }

        return map;

    }

    private static float[,] GenerateCorHelperMap(int localSizeX, int localSizeY, float[,] hHelperMap, float[,] vHelperMap)
    {
        //throw new NotImplementedException();
        float[,] map = new float[localSizeX, localSizeY];
        for (int x = 0; x < localSizeX; x++)
        {
            for (int y = 0; y < localSizeY; y++)
            {
                float newVar = (hHelperMap[x, y] + vHelperMap[x, y]);
                if (newVar > 1f)
                {
                    newVar = 1f;
                }
                map[x, y] = newVar;
            }
        }

        return map;
    }

    private static float[,] GenerateHorHelperMap(int localSizeX, int localSizeY )
    {
        //throw new NotImplementedException();
        int max = localSizeY / 2;
        float[,] map = new float[localSizeX, localSizeY];
        for (int x = 0; x < localSizeX; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < localSizeY; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount--;
                }
            }
        }

        return map;
    }

    private static float[,] GenerateVertHelperMap(int localSizeX, int localSizeY)
    {

        int max = localSizeY / 2;
        float[,] map = new float[localSizeX, localSizeY];
        for (int y = 0; y < localSizeY; y++)
        {
            float hCount = 0f;
            for (int x = 0; x < localSizeX; x++)
            {
                if (x <= max)
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount / (float)max;

                    map[x, y] = toFloat;
                    hCount--;
                }
            }
        }

        return map;
    }










}
