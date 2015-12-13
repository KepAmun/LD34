﻿using UnityEngine;
using System.Collections.Generic;

public class Tile_Grass : Tile
{
    protected override void Awake()
    {
        base.Awake();

        Health = 2;

        Type = TileType.Grass;
    }


    public override void Advance()
    {
        base.Advance();

        Health--;
    }

    public override void CheckHealth()
    {
        base.CheckHealth();

        if(Health <= 0)
        {
            ChangeTo(TileType.Mud);
        }
    }
}