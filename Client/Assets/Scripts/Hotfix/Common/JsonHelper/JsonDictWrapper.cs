using System.Collections.Generic;

[System.Serializable]
public class JsonDictWrapper<TKey, TValue>
{
    [System.Serializable]
    public class KeyValuePair
    {
        public TKey key;
        public TValue value;
    }

    public List<KeyValuePair> items = new List<KeyValuePair>();
}