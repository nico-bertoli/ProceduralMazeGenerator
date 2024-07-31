using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour {
    
    #region ============================================================================================= Private Fields
    
    [SerializeField] private float verticalPadding = 0.2f;
    [SerializeField] private float horizzontalPadding = 0.1f;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void LookAtRectangularObject(Vector3 topLeftPosition, int height, int width) {
        transform.position = new Vector3(topLeftPosition.x + width / 2f - 0.5f, transform.position.y, topLeftPosition.z - height / 2f + 0.5f);
        AdjustCameraSize(height, width);
    }

    #endregion Public Methods
    #region ============================================================================================= Private Methods
    private void AdjustCameraSize(int height, int width)
    {
        // formulas source: https://www.youtube.com/watch?v=3xXlnSetHPM&ab_channel=PressStart

        if (height < width)
        {
            FixWidth(width, horizzontalPadding);

            // after widht fix, maze could still be out of frame
            if (Camera.main.orthographicSize < height / 2f)
                FixHeight(height, verticalPadding);
        }
        else
            FixHeight(height, horizzontalPadding);
    }

    private void FixHeight(float nRows, float bordersPadding) {
        Camera.main.orthographicSize = nRows / 2 + verticalPadding;
        AddPadding((float)bordersPadding);
    }

    private void FixWidth(float nCol, float bordersPadding) {
        Camera.main.orthographicSize = nCol * Screen.height / Screen.width / 2f + horizzontalPadding;
        AddPadding(bordersPadding);
    }

    private void AddPadding(float bordersPadding) {

        if (bordersPadding == 0)
            return;

        Camera.main.orthographicSize += bordersPadding;
    }
    
    #endregion Private Methods
}
