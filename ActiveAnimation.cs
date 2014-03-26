using AnimationOrTween;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Active Animation"), RequireComponent(typeof(Animation))]
public class ActiveAnimation : IgnoreTimeScale
{
    public string callWhenFinished;
    public GameObject eventReceiver;
    private Animation mAnim;
    private AnimationOrTween.Direction mDisableDirection;
    private AnimationOrTween.Direction mLastDirection;
    private bool mNotify;
    public OnFinished onFinished;

    private void Play(string clipName, AnimationOrTween.Direction playDirection)
    {
        if (this.mAnim != null)
        {
            base.enabled = true;
            this.mAnim.enabled = false;
            if (playDirection == AnimationOrTween.Direction.Toggle)
            {
                playDirection = (this.mLastDirection == AnimationOrTween.Direction.Forward) ? AnimationOrTween.Direction.Reverse : AnimationOrTween.Direction.Forward;
            }
            if (string.IsNullOrEmpty(clipName))
            {
                if (!this.mAnim.isPlaying)
                {
                    this.mAnim.Play();
                }
            }
            else if (!this.mAnim.IsPlaying(clipName))
            {
                this.mAnim.Play(clipName);
            }
            foreach (AnimationState state in this.mAnim)
            {
                if (string.IsNullOrEmpty(clipName) || (state.name == clipName))
                {
                    float num = Mathf.Abs(state.speed);
                    state.speed = num * ((float)playDirection);
                    if ((playDirection == AnimationOrTween.Direction.Reverse) && (state.time == 0f))
                    {
                        state.time = state.length;
                    }
                    else if ((playDirection == AnimationOrTween.Direction.Forward) && (state.time == state.length))
                    {
                        state.time = 0f;
                    }
                }
            }
            this.mLastDirection = playDirection;
            this.mNotify = true;
            this.mAnim.Sample();
        }
    }

    public static ActiveAnimation Play(Animation anim, AnimationOrTween.Direction playDirection)
    {
        return Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
    }

    public static ActiveAnimation Play(Animation anim, string clipName, AnimationOrTween.Direction playDirection)
    {
        return Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
    }

    public static ActiveAnimation Play(Animation anim, string clipName, AnimationOrTween.Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
    {
        if (!NGUITools.GetActive(anim.gameObject))
        {
            if (enableBeforePlay != EnableCondition.EnableThenPlay)
            {
                return null;
            }
            NGUITools.SetActive(anim.gameObject, true);
            UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                componentsInChildren[index].Refresh();
                index++;
            }
        }
        ActiveAnimation component = anim.GetComponent<ActiveAnimation>();
        if (component == null)
        {
            component = anim.gameObject.AddComponent<ActiveAnimation>();
        }
        component.mAnim = anim;
        component.mDisableDirection = (AnimationOrTween.Direction) disableCondition;
        component.eventReceiver = null;
        component.callWhenFinished = null;
        component.onFinished = null;
        component.Play(clipName, playDirection);
        return component;
    }

    public void Reset()
    {
        if (this.mAnim != null)
        {
            foreach (AnimationState state in this.mAnim)
            {
                if (this.mLastDirection == AnimationOrTween.Direction.Reverse)
                {
                    state.time = state.length;
                }
                else if (this.mLastDirection == AnimationOrTween.Direction.Forward)
                {
                    state.time = 0f;
                }
            }
        }
    }

    private void Update()
    {
        float num = base.UpdateRealTimeDelta();
        if (num != 0f)
        {
            if (this.mAnim != null)
            {
                bool flag = false;
                foreach (AnimationState state in this.mAnim)
                {
                    if (this.mAnim.IsPlaying(state.name))
                    {
                        float num2 = state.speed * num;
                        state.time += num2;
                        if (num2 < 0f)
                        {
                            if (state.time > 0f)
                            {
                                flag = true;
                            }
                            else
                            {
                                state.time = 0f;
                            }
                        }
                        else if (state.time < state.length)
                        {
                            flag = true;
                        }
                        else
                        {
                            state.time = state.length;
                        }
                    }
                }
                this.mAnim.Sample();
                if (!flag)
                {
                    base.enabled = false;
                    if (this.mNotify)
                    {
                        this.mNotify = false;
                        if (this.onFinished != null)
                        {
                            this.onFinished(this);
                        }
                        if ((this.eventReceiver != null) && !string.IsNullOrEmpty(this.callWhenFinished))
                        {
                            this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                        }
                        if ((this.mDisableDirection != AnimationOrTween.Direction.Toggle) && (this.mLastDirection == this.mDisableDirection))
                        {
                            NGUITools.SetActive(base.gameObject, false);
                        }
                    }
                }
            }
            else
            {
                base.enabled = false;
            }
        }
    }

    public bool isPlaying
    {
        get
        {
            if (this.mAnim != null)
            {
                foreach (AnimationState state in this.mAnim)
                {
                    if (this.mAnim.IsPlaying(state.name))
                    {
                        if (this.mLastDirection != AnimationOrTween.Direction.Forward)
                        {
                            if (this.mLastDirection != AnimationOrTween.Direction.Reverse)
                            {
                                return true;
                            }
                            if (state.time > 0f)
                            {
                                return true;
                            }
                        }
                        else if (state.time < state.length)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public delegate void OnFinished(ActiveAnimation anim);
}

