using UnityEngine;
using System.Collections.Generic;

public class PartyMapBuilder : MonoBehaviour
{
    [Header("Prefab Configuration")]
    public GameObject spacePrefab;

    [Header("Proximity Settings")]
    public float detectionRadius = 3.5f;

    [Header("Live Editor Track")]
    public List<PartySpace> editorSpaces = new List<PartySpace>();
}