using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallObj : MonoBehaviour
{
    //======================================== fields
    private MeshRenderer meshRenderer;
    private float witdh;

    //======================================== methods
    public virtual void SetWidth(float _width) {
        witdh = _width;
        transform.localScale = new Vector3(1+_width,transform.localScale.y,_width);
    }
    public void SetLength(float _l) {
        transform.localScale = new Vector3(_l + witdh, transform.localScale.y, transform.localScale.z);
    }
    /// <summary>
    /// enables/disables wall mesh
    /// </summary>
    /// <param name="_active"></param>
    public void SetMeshActive(bool _active) {
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = _active;
    }
}
