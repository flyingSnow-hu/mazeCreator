using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]MeshMazeView mazeView;
    public void Create()
    {
        IMazeModel model = new FreeMazeModel();
        mazeView.Draw(model);
    }
}
