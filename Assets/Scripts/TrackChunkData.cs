using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TrackChunkData")]
public class TrackChunkData : ScriptableObject
{
    public enum GapPlacement
    {
        Start,
        Middle,
        End,
        NoGap,
        Blank
    }

    public float chunkLength = 200.0f;

    public GameObject[] trackChunks;
    public GapPlacement gapLocation;
}
