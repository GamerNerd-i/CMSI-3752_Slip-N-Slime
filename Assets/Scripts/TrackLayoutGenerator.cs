using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLayoutGenerator : MonoBehaviour
{
    public TrackChunkData[] trackChunkData;
    public TrackChunkData firstChunk;

    private TrackChunkData previousChunk;

    public Vector3 spawnOrigin;

    private Vector3 spawnPosition;
    public int chunksToSpawn = 5;

    private void OnEnable()
    {
        TriggerExit.OnChunkExited += PickAndSpawnChunk;
    }

    private void OnDisable()
    {
        TriggerExit.OnChunkExited -= PickAndSpawnChunk;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PickAndSpawnChunk();
        }
    }

    private void Start()
    {
        previousChunk = firstChunk;

        for (int i = 0; i < chunksToSpawn; i++)
        {
            PickAndSpawnChunk();
        }
    }

    TrackChunkData PickNextChunk()
    {
        List<TrackChunkData> allowedChunks = new List<TrackChunkData>();
        TrackChunkData.GapPlacement bannedGapPlacement = previousChunk.gapLocation;

        spawnPosition = spawnPosition + (Vector3.forward * previousChunk.chunkLength);

        foreach (TrackChunkData block in trackChunkData)
        {
            if (block.gapLocation != bannedGapPlacement)
            {
                allowedChunks.Add(block);
            }
        }

        return allowedChunks[Random.Range(0, allowedChunks.Count)];
    }

    void PickAndSpawnChunk()
    {
        TrackChunkData spawnNext = PickNextChunk();

        GameObject objectFromChunk = spawnNext.trackChunks[
            Random.Range(0, spawnNext.trackChunks.Length)
        ];
        previousChunk = spawnNext;
        Instantiate(objectFromChunk, spawnPosition + spawnOrigin, Quaternion.identity);
    }

    public void UpdateSpawnOrigin(Vector3 originDelta)
    {
        spawnOrigin += originDelta;
    }
}
