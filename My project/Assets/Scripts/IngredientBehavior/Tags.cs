using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    public List<string> tags;
    public void AddTag(string tag) => tags.Add(tag);

    public bool HasTag(string tag) => tags.Contains(tag);
}