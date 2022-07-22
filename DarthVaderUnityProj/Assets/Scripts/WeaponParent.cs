using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    [SerializeField]
    private GameObject _tip;
    [SerializeField]
    private GameObject _base;
    [SerializeField]
    private GameObject _trailMesh;
    [SerializeField]
    private int _trailFrameLength;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;
    private int _framecount;
    private Vector3 _previousTipPosition;
    private Vector3 _previousBasePosition;

    private const int NUM_VERTICES = 12;
    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
        _trailMesh.GetComponent<MeshFilter>().mesh= _mesh;
        
        _vertices = new Vector3[_trailFrameLength * NUM_VERTICES];
        _triangles = new int[_vertices.Length];

        _previousTipPosition = _tip.transform.position;
        _previousBasePosition = _base.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(_framecount == (_trailFrameLength * NUM_VERTICES))
        {
            _framecount = 0;
        }
        
        _vertices[_framecount] = base.transform.position;
        _vertices[_framecount + 1] = _tip.transform.position;
        _vertices[_framecount + 2] = _previousTipPosition;

        _vertices[_framecount + 3] = _base.transform.position;
        _vertices[_framecount + 4] = _previousTipPosition;
        _vertices[_framecount + 5] = _tip.transform.position;

        _vertices[_framecount + 6] = _previousTipPosition;
        _vertices[_framecount + 7] = _base.transform.position;
        _vertices[_framecount + 8] = _previousBasePosition;

        _vertices[_framecount + 9] = _previousTipPosition;
        _vertices[_framecount + 10] = _previousBasePosition;
        _vertices[_framecount + 11] = _base.transform.position;

        _triangles[_framecount] = _framecount;
        _triangles[_framecount + 1] = _framecount + 1;
        _triangles[_framecount + 2] = _framecount + 2;
        _triangles[_framecount + 3] = _framecount + 3;
        _triangles[_framecount + 4] = _framecount + 4;
        _triangles[_framecount + 5] = _framecount + 5;
        _triangles[_framecount + 6] = _framecount + 6;
        _triangles[_framecount + 7] = _framecount + 7;
        _triangles[_framecount + 8] = _framecount + 8;
        _triangles[_framecount + 9] = _framecount + 9;
        _triangles[_framecount + 10] = _framecount + 10;
        _triangles[_framecount + 11] = _framecount + 11;

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _previousTipPosition = _tip.transform.position;
        _previousBasePosition = _base.transform.position;

        _framecount += NUM_VERTICES;
    }
}
