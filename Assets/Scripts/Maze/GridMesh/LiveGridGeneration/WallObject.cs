using System;
using UnityEngine;

public class WallObject : MonoBehaviour
{
    #region ============================================================================================= Private Fields

    private MeshRenderer meshRenderer;
    private Vector3 localScale => transform.localScale;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void SetWidth(float width) => transform.localScale = new Vector3(localScale.x, localScale.y,width);
    public void SetLength(float length) => transform.localScale = new Vector3(length , localScale.y, localScale.z);

    public void SetHeight(float height) => transform.localScale = new Vector3(localScale.x, height, localScale.z);

    public void SetPosition(float x, float z) => transform.position = new Vector3(x, 0, z);

    public void SetMeshActive(bool _active) {
        
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        
        meshRenderer.enabled = _active;
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private void Awake()
    {
        SetHeight(Settings.Instance.MazeGenerationSettings.WallsHeight);
    }

    #endregion Private Methods
}
