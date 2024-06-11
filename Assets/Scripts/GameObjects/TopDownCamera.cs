using UnityEngine;

/// <summary>
/// Adapts to maze size
/// </summary>
[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour {
    
    #region ============================================================================================= Private Fields
    
    [SerializeField] float topBottomMargin = 0.2f;
    [SerializeField] float leftRightMargin = 0.1f;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void CenterPosition(Vector3 topLeftCelPos, int height, int width) {
        transform.position = new Vector3(topLeftCelPos.x + width / 2f - 0.5f, transform.position.y, topLeftCelPos.z - height / 2f + 0.5f);
    }
    
    public void AdjustCameraSize(int height, int width) {
        
        // formulas source: https://www.youtube.com/watch?v=3xXlnSetHPM&ab_channel=PressStart
        bool wasAdjustmentVertical = true;

        if (height < width) {
            FixWidth(width);

            // after widht fix, the rectangle could still be out of frame
            if (Camera.main.orthographicSize < height / 2f)
                FixHeight(height);
            else
                wasAdjustmentVertical = false;
        }
        else {
            FixHeight(height);
        }

        if (wasAdjustmentVertical)
            AddMargin(topBottomMargin);
        else
            AddMargin(leftRightMargin);
    }
    #endregion Public Methods
    #region ============================================================================================= Public Methods
    
    private void FixHeight(float nRows) {
        Camera.main.orthographicSize = nRows / 2 + topBottomMargin;
    }
    private void FixWidth(float nCol) {
        Camera.main.orthographicSize = nCol * Screen.height / Screen.width / 2f + leftRightMargin;
    }
    private void AddMargin(float margin) {
        Camera.main.orthographicSize += margin;
    }
    
    #endregion Public Methods
}
