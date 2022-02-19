using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Direction
{
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8
}

[RequireComponent(typeof(Renderer))]
public class SquareCellView : MonoBehaviour
{
    [SerializeField]private Texture2D[] gridTexs;
    [SerializeField]public TextMesh branch;
    [SerializeField]public TextMesh distance;

    public SquareCellModel Model {get;private set;}

    private new Renderer renderer;

    private CellState _state = CellState.Unconnected;
    public CellState state
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
            //     case CellState.Solution:
            //         renderer.material.SetColor("_MainColor", Color.yellow);
            //         break;
            //     case CellState.Connected:
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

    public void SetPos(int x, int y)
    {
        Model = new SquareCellModel(x, y);
    }

    public void SetWays(Direction ways)
    {
        renderer.material.SetTexture("_MainTex", gridTexs[(int)(~ways) & 0xf]);
    }

    public void SetSolution(bool isSolution)
    {
        Model.IsSolution = isSolution;
        if (isSolution)
        {
            renderer.material.SetColor("_MainColor", new Color(0.5f, 0.8f, 0.5f));
        }else
        {
            renderer.material.SetColor("_MainColor", Color.white);
        }
    }
}
