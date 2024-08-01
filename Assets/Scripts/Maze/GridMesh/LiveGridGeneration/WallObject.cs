using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class WallObject : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    [SerializeField] private MeshRenderer meshRenderer;
    private Vector3 localScale => transform.localScale;

    #endregion Private Fields
    #region ============================================================================================= Methods

    //-------------- public 
    public void SetWidth(float width) => transform.localScale = new Vector3(localScale.x, localScale.y,width);
    public void SetLength(float length) => transform.localScale = new Vector3(length , localScale.y, localScale.z);
    public void SetHeight(float height) => transform.localScale = new Vector3(localScale.x, height, localScale.z);
    public void SetPosition(float x, float z) => transform.position = new Vector3(x, 0, z);

    public void SetMeshActive(bool _active) => meshRenderer.enabled = _active;


    //-------------- private 
    private void Awake() => SetHeight(Settings.Instance.MazeGenerationSettings.WallsHeight);

    #endregion Methods



}
