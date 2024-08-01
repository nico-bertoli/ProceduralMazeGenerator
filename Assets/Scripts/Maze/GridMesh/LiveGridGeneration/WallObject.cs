using UnityEngine;

public class WallObject : MonoBehaviour
{
    //-------------- properties
    private Vector3 localScale => transform.localScale;

    //-------------- public methods
    public void SetWidth(float width) => transform.localScale = new Vector3(localScale.x, localScale.y,width);
    public void SetLength(float length) => transform.localScale = new Vector3(length , localScale.y, localScale.z);
    public void SetHeight(float height) => transform.localScale = new Vector3(localScale.x, height, localScale.z);
    public void SetPosition(float x, float z) => transform.position = new Vector3(x, 0, z);

    //-------------- private methods
    private void Awake() => SetHeight(Settings.Instance.MazeGenerationSettings.WallsHeight);
}
