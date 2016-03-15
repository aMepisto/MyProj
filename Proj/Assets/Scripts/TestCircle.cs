// UTF8 & LF (유티에프:)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCircle : MonoBehaviour
{
    #region variables
    private readonly string SHADER_NAME = "Particles/Additive";
    private readonly int CIRCLE_RADIUS = 200;
    private readonly int POINT_COUNT = 50;
    private readonly float ROTATE_ANGLE = 5f;
    private float _circleRadius;
    private float _speed;
    private bool _isUpdate;
    private bool _reverse;
    private float _rotateAngle;
    private bool _isUseLineRenderer;
    RectTransform _rectTransform;

    // note : Use LineRenderer
    private int _lineSegments;
    private float _lineStartWidth;
    private float _lineEndWidth;
    private float _lineAngle;
    LineRenderer _lineRenderer;

    // note : Use MeshRenderer
    Quaternion _meshQuaternion = Quaternion.identity;
    private int _meshNumOfPoints;
    private Mesh _circleMesh = null;
    public List<Vector3> _meshVertexList = new List<Vector3>();
    private List<int> _meshTriangleList = new List<int>();
    private MeshRenderer _circleMeshRenderer = new MeshRenderer();
    private MeshFilter _circlaMeshFilter = new MeshFilter();
    #endregion variables

    private void Awake()
    {
        this.gameObject.layer = LayerMask.NameToLayer("UI");

        Refresh();
    }

    private void Refresh()
    {
        this._lineSegments = POINT_COUNT;
        this._lineStartWidth = 0.1f;
        this._lineEndWidth = 0.1f;
        this._meshNumOfPoints = POINT_COUNT;
        this._rotateAngle = ROTATE_ANGLE;
        this._isUpdate = false;
        this._reverse = false;
        this._isUseLineRenderer = false;
        this._circleRadius = CIRCLE_RADIUS;
        this._speed = 100;
    }

    private void Start()
    {
        if (_rectTransform == null)
            _rectTransform = this.GetComponent<RectTransform>();

        InitLineRenderer();
        InitMeshRenderer();
    }

    private void InitLineRenderer()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (_lineRenderer != null)
        {
            _lineRenderer.material = new Material(Shader.Find(SHADER_NAME));
            _lineRenderer.SetVertexCount(_lineSegments + 1);
            _lineRenderer.SetWidth(_lineStartWidth, _lineEndWidth);
            _lineRenderer.useWorldSpace = false;
        }
    }

    private void InitMeshRenderer()
    {
        float angleStep = 360.0f / (float)_meshNumOfPoints;
        _meshQuaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
        _circleMeshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        if (_circleMeshRenderer == null)
            _circleMeshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        _circleMeshRenderer.material = new Material(Shader.Find(SHADER_NAME));

        _circlaMeshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (_circlaMeshFilter == null)
            _circlaMeshFilter = this.gameObject.AddComponent<MeshFilter>();

        CreateMeshCircle();
    }

    private void Update()
    {
        if (_isUpdate == false)
            return;

        if (_isUseLineRenderer)
            UpdateLineRendererPoints();
        else
            UpdateMeshRendererPoints();

        if (_isUpdate)
        {
            _rectTransform.Rotate(Vector3.forward, _rotateAngle);
            _circleRadius -= Time.deltaTime * _speed;
            if (_reverse)
            {
                _reverse = false;
                _speed *= -1f;
                if (_circleRadius < 0)
                {
                    _circleRadius = CIRCLE_RADIUS * -1f;
                }
                else if (0 < _circleRadius)
                {
                    _circleRadius = CIRCLE_RADIUS;
                }
            }
            else
            {
                if (CIRCLE_RADIUS <= Mathf.Abs(_circleRadius))
                {
                    _rotateAngle *= -1f;
                    _reverse = true;
                }
            }
        }
    }

    #region Use MeshRenderer
    private void CreateMeshCircle()
    {
        MakeFirstTriangle();
        AddTriandleIndices();

        for (int i = 0; i < _meshNumOfPoints - 1; i++)
        {
            _meshTriangleList.Add(0);
            _meshTriangleList.Add(_meshVertexList.Count - 1);
            _meshTriangleList.Add(_meshVertexList.Count);
            _meshVertexList.Add(_meshQuaternion * _meshVertexList[_meshVertexList.Count - 1]);
        }
    }

    private void UpdateMeshRendererPoints()
    {
        ReSetFirstTriangle();
        for (int i = 1; i < _meshNumOfPoints; i++)
        {
            _meshVertexList[i + 2] = _meshQuaternion * _meshVertexList[i + 1];
        }

        if (_circleMesh == null)
            _circleMesh = new Mesh();
        _circleMesh.vertices = _meshVertexList.ToArray();
        _circleMesh.triangles = _meshTriangleList.ToArray();

        //Debug2.Assert(_circlaMeshFilter != null);
        _circlaMeshFilter.mesh = _circleMesh;
        _circlaMeshFilter.name = "testMeshCircle";
    }

    private void MakeFirstTriangle()
    {
        // Make first triangle.
        //Debug2.Assert(_meshVertexList != null);
        _meshVertexList.Add(new Vector3(0.0f, 0.0f, -1000f));           // 1. Circle center.
        _meshVertexList.Add(new Vector3(0.0f, _circleRadius, 0.0f));    // 2. First vertex on circle outline (_circleRadius = 0.5f)
        _meshVertexList.Add(_meshQuaternion * _meshVertexList[1]);      // 3. First vertex on circle outline rotated by angle)
    }

    private void ReSetFirstTriangle()
    {
        // Reset first triangle.
        //Debug2.Assert(_meshVertexList != null);
        _meshVertexList[0] = new Vector3(0.0f, 0.0f, -1000f);           // 1. Circle center.
        _meshVertexList[1] = new Vector3(0.0f, _circleRadius, -1000f);  // 2. First vertex on circle outline (_circleRadius = 0.5f)
        _meshVertexList[2] = _meshQuaternion * _meshVertexList[1];      // 3. First vertex on circle outline rotated by angle)
    }

    private void AddTriandleIndices()
    {
        // Add triangle indices.
        //Debug2.Assert(_meshTriangleList != null);
        _meshTriangleList.Add(0);
        _meshTriangleList.Add(1);
        _meshTriangleList.Add(2);
    }
    #endregion Use MeshRenderer

    #region Use LineRenderer
    private void UpdateLineRendererPoints()
    {
        if (_circleRadius < 0)
            _lineAngle = -20f;
        else if (0 < _circleRadius)
            _lineAngle = 20f;

        float x;
        float y;
        float z = -1000f;

        for (int i = 0; i < (_lineSegments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * _lineAngle) * _circleRadius;
            y = Mathf.Cos(Mathf.Deg2Rad * _lineAngle) * _circleRadius;

            _lineRenderer.SetPosition(i, new Vector3(x, y, z));
            _lineAngle += (360f / _lineSegments);
        }
    }
    #endregion Use LineRenderer

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button(_isUseLineRenderer == true ? "use mesh" : "use line"))
        {
            _lineRenderer.enabled = false;
            _circleMeshRenderer.enabled = false;

            _isUseLineRenderer = !_isUseLineRenderer;
            if (_isUseLineRenderer)
                _lineRenderer.enabled = true;
            else
                _circleMeshRenderer.enabled = true;
        }

        if (GUILayout.Button(_isUpdate == true ? "Stop" : "Play"))
        {
            _isUpdate = !_isUpdate;
            if (_isUpdate == false)
                Debug.LogError("CircleRadius : " + _circleRadius);
        }

        if (GUILayout.Button("ScreenSize"))
        {
            Resolution[] resolutions = Screen.resolutions;
            foreach (Resolution res in resolutions)
            {
                Debug.LogError(res.width + "x" + res.height);
            }
        }
    }
#endif
}