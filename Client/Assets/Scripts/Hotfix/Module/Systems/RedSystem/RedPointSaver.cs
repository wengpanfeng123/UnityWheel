using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPointSaver : MonoBehaviour
{
    private const string SAVE_KEY = "RedPointStates";

    /*
    void OnEnable()
    {
        LoadStates();
        RedPointSystem.OnAnyNodeChanged += SaveStates;
    }

    private void LoadStates()
    {
        var json = PlayerPrefs.GetString(SAVE_KEY);
        var states = JsonUtility.FromJson<RedPointStateCollection>(json);
        
        foreach (var state in states.list)
        {
            RedPointSystem.SetNodeValue(state.path, state.value, state.type);
        }
    }

    private void SaveStates()
    {
        var states = new RedPointStateCollection();
        
        foreach (var node in RedPointSystem.AllNodes)
        {
            states.list.Add(new RedPointState{
                path = node.path,
                value = node.value,
                type = node.type
            });
        }
        
        PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(states));
    }
    */
}