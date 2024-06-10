using UnityEngine;

public class WallObj : MonoBehaviour
{
    #region ============================================================================================= Private Fields

    private MeshRenderer meshRenderer;
    private float witdh;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods

    public virtual void SetWidth(float _width) {
        witdh = _width;
        transform.localScale = new Vector3(1+_width,transform.localScale.y,_width);
    }
    public void SetLength(float _l) {
        transform.localScale = new Vector3(_l + witdh, transform.localScale.y, transform.localScale.z);
    }
    
    public void SetMeshActive(bool _active) {
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = _active;
    }
    
    #endregion Public Methods
}
