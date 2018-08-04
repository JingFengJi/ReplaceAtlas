//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example script of how a button can be scaled visibly when the mouse hovers over it or it gets pressed.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button ScaleEx")]
public class UIButtonScaleEx : MonoBehaviour
{
    public Transform tweenTarget;
    public Vector3 hover = new Vector3(1f, 1f, 1f);
    public Vector3 pressed = new Vector3(0.95f, 0.95f, 0.95f);
    public Vector3 relesed = new Vector3(1.05f, 1.05f, 1.05f);
    public float pressedDuration = 0.1f;
    public float relesedDuration = 0.05f;
    public float turnDuration = 0.025f;
    Vector3 mScale;
    bool mStarted = false;
    EventDelegate call;
    void Start()
    {
        if (!mStarted)
        {
            mStarted = true;
            if (tweenTarget == null) tweenTarget = transform;
            mScale = tweenTarget.localScale;
            call = new EventDelegate(RelasedFinish);
        }
    }

    void OnPress(bool isPressed)
    {
        if (enabled)
        {
            if (!mStarted) Start();
            isPress = isPressed;
            if (isPress)
            {
                tweenTarget.localScale = Vector3.one;
            }
            TweenScale tweenScale = TweenScale.Begin(tweenTarget.gameObject, isPressed ? pressedDuration : relesedDuration, isPressed ? Vector3.Scale(mScale, pressed) :
            (UICamera.IsHighlighted(gameObject) ? Vector3.Scale(mScale, relesed) : mScale));
            tweenScale.method = UITweener.Method.EaseInOut;
            if (!isPress)
            {
                tweenScale.SetOnFinished(call);
            }
        }
    }
    bool isPress;
    void RelasedFinish()
    {
        if (!isPress)
        {
            TweenScale.Begin(tweenTarget.gameObject, turnDuration, Vector3.one);
        }
    }
}
