using UnityEngine;
public abstract class Node
{
    public Transform transform;
    public Transform root;

    protected string VarDefine;
    public Node(Transform transform, Transform root)
    {
        this.transform = transform;
        this.root = root;
        ToLowerFirstLetter();
    }
    
    void ToLowerFirstLetter()
    {
        string input = transform.name;
        if (string.IsNullOrEmpty(input))
        {
            return ;
        }

        // 将首字母小写，并与剩余部分拼接
        VarDefine ="_"+char.ToLower(input[0]) + input.Substring(1);
    }
    
    public abstract string getVarDefine();

    public abstract string getLocalFind();
}