using Xicheng.ReferencePool;
using UnityEngine;


class TestRefPool
{
    public void Test()
    {
        TeacherData teacherData1 = RefPool.Acquire<TeacherData>();
        teacherData1.Name = "aaa";
        teacherData1.Age = 30;
        // _refPool.Release(teacherData1);
        TeacherData teacherData2 = RefPool.Acquire<TeacherData>();
        teacherData2.Name = "bbb";
        teacherData2.Age = 20;
        // Debug.Log($"当前引用的数量:{_refPool.GetCurrUsingRefCount<TeacherData>()}");
        // Debug.Log($"请求引用的总数量:{_refPool.GetAcquireRefCount<TeacherData>()}");
        // Debug.Log($"释放引用的总数量:{_refPool.GetReleaseRefCount<TeacherData>()}");
        // Debug.Log($"添加引用的总数量:{_refPool.GetAddRefCount<TeacherData>()}");
        // Debug.Log($"移除引用的总数量:{_refPool.GetRemoveRefCount<TeacherData>()}");
    }
}

public class TeacherData : IReference
{
    public string Name;
    public int Age;

    public void Clear()
    {
        Name = string.Empty;
        Age = 0;
    }
}