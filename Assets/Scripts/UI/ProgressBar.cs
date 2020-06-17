using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    [SerializeField] RectTransform leftProgressBar;
    [SerializeField] RectTransform centerProgressBar;
    [SerializeField] RectTransform rightProgressBar;
    [SerializeField] RectTransform leftProgressBarFiller;
    [SerializeField] RectTransform centerProgressBarFiller;
    [SerializeField] RectTransform rightProgressBarFiller;

    [SerializeField] int centerWidth = 256;
    [SerializeField] int cornersWidth = 32;
    [SerializeField] float value = 0.5f;

    public float Value {
        set {
            this.value = value;
            UpdateRect();
        }
    }

    protected virtual void Start() {
        leftProgressBar.anchoredPosition -= new Vector2(cornersWidth/2f + centerWidth/2f, 0);
        rightProgressBar.anchoredPosition += new Vector2(cornersWidth/2f + centerWidth/2f, 0);
        centerProgressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, centerWidth);
        UpdateRect();
    }

    void UpdateRect() {
        int currentWidth = (int) ((centerWidth + 2 * cornersWidth) * value);
        leftProgressBarFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth < cornersWidth ? currentWidth : cornersWidth);
        
        currentWidth -= cornersWidth;
        if (currentWidth < 0)
            currentWidth = 0;
        
        centerProgressBarFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth < centerWidth ? currentWidth : centerWidth);
        
        currentWidth -= centerWidth;
        if (currentWidth < 0)
            currentWidth = 0;
        
        rightProgressBarFiller.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth < cornersWidth ? currentWidth : cornersWidth);
    }
}