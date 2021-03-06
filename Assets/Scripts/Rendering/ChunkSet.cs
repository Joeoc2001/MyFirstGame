﻿using Algebra;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;

namespace SDFRendering
{
    public class ChunkSet : MonoBehaviour, IEnumerable<Chunk>
    {
        public Vector3Int BaseIndex { get; set; }

        private readonly TwoWayDict<Vector3Int, Chunk> _chunks = new TwoWayDict<Vector3Int, Chunk>();

        public delegate void ChunkEvent(ChunkSet set, Chunk chunk, Vector3Int index);
        public event ChunkEvent OnChunkAdded;
        public event ChunkEvent OnChunkRemoved;

        public int Count { get => _chunks.Count; }

        public bool TryGetChunk(Vector3Int index, out Chunk chunk)
        {
            return _chunks.TryGetValue(index, out chunk);
        }

        public bool TryGetIndex(Chunk chunk, out Vector3Int index)
        {
            return _chunks.TryGetValue(chunk, out index);
        }

        public bool HasChunk(Vector3Int index)
        {
            return _chunks.ContainsKey(index);
        }

        public Vector3Int? GetChunkIndex(Chunk chunk)
        {
            if (TryGetIndex(chunk, out Vector3Int index))
            {
                return index;
            }

            return null;
        }

        public Chunk GetChunkByIndex(Vector3Int index)
        {
            if (TryGetChunk(index, out Chunk chunk))
            {
                return chunk;
            }

            return null;
        }

        public Vector3Int? GetChunkOffset(Chunk chunk)
        {
            Vector3Int? indexQuery = GetChunkIndex(chunk);
            if (!indexQuery.HasValue)
            {
                return null;
            }

            Vector3Int index = indexQuery.Value;
            return index - BaseIndex;
        }

        public void SetChunk(Vector3Int index, Chunk chunk)
        {
            // Check index isn't already set
            RemoveChunk(index);

            // Add new chunk to data structures
            _chunks.Add(index, chunk);

            // Set fields
            Vector3 position = new Vector3(index.x, index.y, index.z) * Chunk.SIZE;
            chunk.SamplingOffset = position;
            chunk.OwningSet = this;

            // Trigger listeners
            OnChunkAdded.Invoke(this, chunk, index);
        }

        public Chunk RemoveChunk(Vector3Int index)
        {
            // If the index isn't in chunks then the chunk doesn't exist
            if (!_chunks.TryGetValue(index, out Chunk chunk))
            {
                return null;
            }

            chunk.OwningSet = null;

            // Remove the chunk from the dictionaries
            _chunks.Remove(index);

            // Trigger listeners
            OnChunkRemoved.Invoke(this, chunk, index);

            return chunk;
        }

        public IEnumerator<Chunk> GetEnumerator()
        {
            return _chunks.GetEnumerator2();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _chunks.GetEnumerator2();
        }
    }
}