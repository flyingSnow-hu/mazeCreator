using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Bound
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8
}

public enum GridState
{
    Unchecked = 0,
    Solution = 1,
    NotRight = 2,
    Blocked = 3
}

[RequireComponent(typeof(Renderer))]
public class Grid : MonoBehaviour
{
    [SerializeField]private Texture2D[] gridTexs;

    private new Renderer renderer;
    public Bound Walls
    {
        get; private set;
    } = Bound.None;
    public Bound Ways
    {
        get; private set;
    } = Bound.None;

    private GridState _state = GridState.Unchecked;
    public GridState state
    {
        get
        {
            return _state;
        } 
        set
        {
            _state = value;
            // switch(_state)
            // {
            //     case GridState.Unchecked:
            //         renderer.material.SetColor("_MainColor", Color.yellow);
            //         break;
            //     case GridState.Solution:
            //         renderer.material.SetColor("_MainColor", Color.green);
            //         break;
            //     case GridState.NotRight:
            //         renderer.material.SetColor("_MainColor", Color.gray);
            //         break;
            //     default:                    
            //         renderer.material.SetColor("_MainColor", Color.white);
            //         break;
            // }
        }
        
    }

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }


    public void AddWall(Bound wall)
    {
        // Walls = (Walls | wall) & ((~Ways) & (Bound)0xf);


        // renderer.material.SetTexture("_MainTex", gridTexs[(int)(~Ways) & 0xf]);
    }


    public void AddWay(Bound way)
    {
        Ways = Ways | way;
        renderer.material.SetTexture("_MainTex", gridTexs[(int)(~Ways) & 0xf]);
    }
}
