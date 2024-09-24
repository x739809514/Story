using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogAsset")]
public class DialogAssets : ScriptableObject
{
    public List<Person> dialogPersons;
}

[Serializable]
public class Person
{
    public string name;
}