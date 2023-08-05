using UnityEngine;

public class UISizeMatcher : MonoBehaviour
{
    [SerializeField] private RectTransform objectToMatch;
    [SerializeField] private bool matchWidth = false;
    [SerializeField] private bool matchHeight = false;
    [SerializeField] private bool matchConstantly = false;
    private RectTransform rectTransform;

    private void Start()
    {
        GetRectTransform();

        DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();
        tracker.Clear();
        tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
    }

    private void Update()
    {
        if (matchConstantly)
        {
            Match();
        }
    }

    void GetRectTransform()
    {
        if (rectTransform == null && transform is RectTransform rt)
        {
            rectTransform = rt;
        }
    }

    public void Match()
    {
        GetRectTransform();

        float w = matchWidth ? objectToMatch.sizeDelta.x : rectTransform.sizeDelta.x;
        float h = matchHeight ? objectToMatch.sizeDelta.y : rectTransform.sizeDelta.y;

        rectTransform.sizeDelta = new Vector2(w, h);
    }
}
