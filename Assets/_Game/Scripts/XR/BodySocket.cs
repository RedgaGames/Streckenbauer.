using UnityEngine;

/// <summary>
/// Represents a body socket with an associated game object and a height ratio.
/// </summary>
[System.Serializable]
public class BodySocket
{
    public GameObject BodySocketObject;
    [Range(0.01f, 1f)] public float HeightRatio;
}
