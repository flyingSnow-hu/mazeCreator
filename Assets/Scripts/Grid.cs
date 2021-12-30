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
    Unconnected = 0,
    Connected = 1,
    Solution = 2,
    NotSolution = 3,
}

[RequireComponent(typeof(Renderer))]
public class Grid : MonoBehaviour
{
    [SerializeField]private Texture2D[] gridTexs;
    [SerializeField]public TextMesh branch;
    [SerializeField]public TextMesh distance;

    private new Renderer renderer;
    public Bound Ways
    {
        get; private set;
    } = Bound.None;

    private GridState _state = GridState.Unconnected;
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
            //     case GridState.Solution:
            //         renderer.material.SetColor("_MainColor", Color.yellow);
            //         break;
            //     case GridState.Connected:
            //         renderer.material.SetColor("_MainColor", Color.white);
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


    public void Reset()
    {
        Ways = Bound.None;
        renderer.material.SetTexture("_MainTex", gridTexs[(int)(~Ways) & 0xf]);
    }


    public void AddWay(Bound way)
    {
        Ways = Ways | way;
        renderer.material.SetTexture("_MainTex", gridTexs[(int)(~Ways) & 0xf]);
    }
}
