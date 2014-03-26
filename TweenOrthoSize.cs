using System;
using UnityEngine;

[RequireComponent(typeof(Camera)), AddComponentMenu("NGUI/Tween/Orthographic Size")]
public class TweenOrthoSize : UITweener
{
    public float from;
    private Camera mCam;
    public float to;

    public static TweenOrthoSize Begin(GameObject go, float duration, float to)
    {
        TweenOrthoSize size = UITweener.Begin<TweenOrthoSize>(go, duration);
        size.from = size.orthoSize;
        size.to = to;
        if (duration <= 0f)
        {
            size.Sample(1f, true);
            size.enabled = false;
        }
        return size;
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        this.cachedCamera.orthographicSize = (this.from * (1f - factor)) + (this.to * factor);
    }

    public Camera cachedCamera
    {
        get
        {
            if (this.mCam == null)
            {
                this.mCam = base.camera;
            }
            return this.mCam;
        }
    }

    public float orthoSize
    {
        get
        {
            return this.cachedCamera.orthographicSize;
        }
        set
        {
            this.cachedCamera.orthographicSize = value;
        }
    }
}

