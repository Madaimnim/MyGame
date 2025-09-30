using System;
using UnityEngine;

public interface IExpData
{
    int Exp{get;set;}
    int Level { get; set; }
    int[] ExpTable { get; }
}
