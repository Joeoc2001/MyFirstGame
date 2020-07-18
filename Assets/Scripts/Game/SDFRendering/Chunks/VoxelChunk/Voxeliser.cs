﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Voxeliser : PointCloudMeshifier
{
    private enum Direction
    {
        XMinus,
        XPlus,
        YMinus,
        YPlus,
        ZMinus,
        ZPlus,
    }

    public static readonly Voxeliser Instance = new Voxeliser();

    public Voxeliser() : base(1, 1)
    {

    }

    protected override void GenerateForNode(Datas space, FeelerNodeSet nodes, Vector3Int index)
    {
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            ConditionallyAddWall(space, nodes, index, dir);
        }
    }

    private void ConditionallyAddWall(Datas space, FeelerNodeSet nodes, Vector3Int index, Direction dir)
    {
        FeelerNode thisNode = nodes[index];

        if (thisNode.Val >= 0) // If outside shape, return
        {
            return;
        }

        Vector3Int otherIndex = index + GetDirectionOffset(dir);
        FeelerNode otherNode = nodes[otherIndex];

        if (otherNode.Val < 0) // If other is inside shape, return
        {
            return;
        }

        // ASSERT: otherNode is outside shape and thisNode is inside shape
        // Therefore a wall should be put between them
        Vector3Int[] faceVectors = GetFaceWithWinding(dir);
        int[] vertexIndices = new int[faceVectors.Length];

        float spacing = nodes.Spacing;

        for(int i = 0; i < faceVectors.Length; i++)
        {
            Vector3Int faceIndex = index + ((faceVectors[i] - new Vector3Int(1, 1, 1)) / 2);
            Vector3 facePosition = (Vector3)thisNode.Pos + ((Vector3)faceVectors[i] * (spacing / 2));

            vertexIndices[i] = GetOrAddVertex(space, faceIndex, facePosition);
        }

        for (int i = 1; i < vertexIndices.Length - 1; i++)
        {
            space.chunkTriangles.Add(vertexIndices[0]);
            space.chunkTriangles.Add(vertexIndices[i]);
            space.chunkTriangles.Add(vertexIndices[i + 1]);
        }
    }

    private int GetOrAddVertex(Datas data, Vector3Int index, Vector3 vector)
    {
        int iChunkVertex;
        if (data.chunkVertexCache.IsSet(index, 0))
        {
            iChunkVertex = data.chunkVertexCache.Get(index, 0);
        }
        else
        {
            iChunkVertex = data.chunkVertices.Count;
            data.chunkVertices.Add(vector);

            data.chunkVertexCache.Set(index, 0, iChunkVertex);
        }
        return iChunkVertex;
    }

    private Vector3Int GetDirectionOffset(Direction dir)
    {
        Vector3Int off;
        switch (dir)
        {
            case Direction.XMinus:
                off = new Vector3Int(-1, 0, 0);
                break;
            case Direction.XPlus:
                off = new Vector3Int(1, 0, 0);
                break;
            case Direction.YMinus:
                off = new Vector3Int(0, -1, 0);
                break;
            case Direction.YPlus:
                off = new Vector3Int(0, 1, 0);
                break;
            case Direction.ZMinus:
                off = new Vector3Int(0, 0, -1);
                break;
            case Direction.ZPlus:
                off = new Vector3Int(0, 0, 1);
                break;
            default:
                throw new NotImplementedException("Somehow another spacial dimension has been queried that doesn't exist :/");
        }
        return off;
    }

    private Vector3Int[] GetFaceWithWinding(Direction dir)
    {
        Vector3Int[] offs; // The set of points for the face and the winding order
        switch (dir)
        {
            case Direction.XMinus:
                offs = new Vector3Int[] {
                    new Vector3Int(-1, 1, 1),
                    new Vector3Int(-1, 1, -1),
                    new Vector3Int(-1, -1, -1),
                    new Vector3Int(-1, -1, 1),
                };
                break;
            case Direction.XPlus:
                offs = new Vector3Int[] {
                    new Vector3Int(1, 1, 1),
                    new Vector3Int(1, -1, 1),
                    new Vector3Int(1, -1, -1),
                    new Vector3Int(1, 1, -1),
                };
                break;
            case Direction.YMinus:
                offs = new Vector3Int[] {
                    new Vector3Int(1, -1, 1),
                    new Vector3Int(-1, -1, 1),
                    new Vector3Int(-1, -1, -1),
                    new Vector3Int(1, -1, -1),
                };
                break;
            case Direction.YPlus:
                offs = new Vector3Int[] {
                    new Vector3Int(1, 1, 1),
                    new Vector3Int(1, 1, -1),
                    new Vector3Int(-1, 1, -1),
                    new Vector3Int(-1, 1, 1),
                };
                break;
            case Direction.ZMinus:
                offs = new Vector3Int[] {
                    new Vector3Int(1, 1, -1),
                    new Vector3Int(1, -1, -1),
                    new Vector3Int(-1, -1, -1),
                    new Vector3Int(-1, 1, -1),
                };
                break;
            case Direction.ZPlus:
                offs = new Vector3Int[] {
                    new Vector3Int(1, 1, 1),
                    new Vector3Int(-1, 1, 1),
                    new Vector3Int(-1, -1, 1),
                    new Vector3Int(1, -1, 1),
                };
                break;
            default:
                throw new NotImplementedException("Somehow another spacial dimension has been queried that doesn't exist :/");
        }
        return offs;
    }
}