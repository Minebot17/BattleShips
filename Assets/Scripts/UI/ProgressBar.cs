using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    [SerializeField] private RectTransform leftProgressBar;
    [SerializeField] private RectTransform centerProgressBar;
    [SerializeField] private RectTransform rightProgressBar;
    [SerializeField] private RectTransform leftProgressBarFiller;
    [SerializeField] private RectTransform centerProgressBarFiller;
    [SerializeField] private RectTransform rightProgressBarFiller;

    [SerializeField] private int centerWidth = 256;
    [SerializeField] private int cornersWidth = 32;
    [SerializeField] private float value = 0.5f;

    public float Value {
        get => value;
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

    private void UpdateRect() {
        int currentWidth = (int) ((centerWidth + 2 * cornersWidth) * value);
        if (!leftProgressBarFiller)
            return;
        
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