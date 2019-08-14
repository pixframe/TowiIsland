using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HeaderToJson : ScriptableObject
{
    [SerializeField] string kidName;
    [SerializeField] int childName;

    public string GetData()
    {
        return $"kid is {kidName} id is {childName}";
    }

    public void SetData(string kid, int child)
    {
        kidName = kid;
        childName = child;
    }
}
