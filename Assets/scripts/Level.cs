using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    public int levelId { get; set; }
    public Rigidbody2D rb { get; set; }
    public charakter cs { get; set; }
    public Vector2 PosStart { get; set; }
    public Vector2 PosEnd { get; set; }
    //public levelState state { get; set; }


    public virtual void displayLevel(GameObject[] levels, int level, Transform grass) { }

    public virtual void generateSection(int seed)
    {

    }
}
