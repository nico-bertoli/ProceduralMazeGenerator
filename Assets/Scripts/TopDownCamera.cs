using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Topdown camera that can automatically frames a rectangle
/// </summary>
[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour {

    //======================================== fields

    /// <summary>
    /// Vertical margin from the top/bottom of the rectangle
    /// </summary>
    [SerializeField] float vertMargin = 0.2f;
    /// <summary>
    /// Orizzontal margin from the left/right edge of the rectangle
    /// </summary>
    [SerializeField] float orizMargin = 0.1f;

    //======================================== methods

    /// <summary>
    /// Centers camera position depending on the rectangle size
    /// </summary>
    /// <param name="_topLeftCelPos">Position of the top left corner of the rectangle</param>
    /// <param name="_height">Rectangle height</param>
    /// <param name="_width">Rectangle width</param>
    public void AdjustCameraPosition(Vector3 _topLeftCelPos, int _height, int _width) {
        transform.position = new Vector3(_topLeftCelPos.x + _width / 2f - 0.5f, transform.position.y, _topLeftCelPos.z - _height / 2f + 0.5f);
    }

    /// <summary>
    /// Sets camera orthographic size depending on rectangle size
    /// </summary>
    /// <param name="_height">Height of the rectangle</param>
    /// <param name="_width">Width of the rectangle</param>
    public void AdjustCameraSize(int _height, int _width) {
        // formulas source: https://www.youtube.com/watch?v=3xXlnSetHPM&ab_channel=PressStart

        bool wasAdjustmentVertical = true;

        if (_height < _width) {
            fixWidth(_width);

            // after widht fix, the rectangle could still be out of frame
            if (Camera.main.orthographicSize < _height / 2f)
                fixHeight(_height);
            else
                wasAdjustmentVertical = false;
        }
        else {
            fixHeight(_height);
        }

        if (wasAdjustmentVertical)
            addMargin(vertMargin);
        else
            addMargin(orizMargin);
    } 

    private void fixHeight(float _nRows) {
        Camera.main.orthographicSize = _nRows / 2 + vertMargin;
    }
    private void fixWidth(float _nCol) {
        Camera.main.orthographicSize = _nCol * Screen.height / Screen.width / 2f + orizMargin;
    }
    private void addMargin(float _margin) {
        Camera.main.orthographicSize += _margin;
    }
}
