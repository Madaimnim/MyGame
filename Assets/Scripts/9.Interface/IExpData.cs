using System;
using UnityEngine;

public interface IExpData
{
    int CurrentExp{get;set;}
    int CurrentLevel { get; set; }
    int[] ExpTable { get; }
}
