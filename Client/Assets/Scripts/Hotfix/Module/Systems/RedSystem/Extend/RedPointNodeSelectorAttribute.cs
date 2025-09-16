using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPointNodeSelectorAttribute : PropertyAttribute
{
    public string Name;
    public RedPointNodeSelectorAttribute(string name)
    {
        Name =name;
    }
 
    public bool ShowSearchBox = true;
}
