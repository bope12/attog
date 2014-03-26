using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
    private Material mMat;
    private Texture2D mTex;
    public string url = "http://www.tasharen.com/misc/logo.png";

    private void OnDestroy()
    {
        if (this.mMat != null)
        {
            UnityEngine.Object.Destroy(this.mMat);
        }
        if (this.mTex != null)
        {
            UnityEngine.Object.Destroy(this.mTex);
        }
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
        STARTCITERATOR iterator = new STARTCITERATOR();
        iterator.FUCKTHIS = this;
        return iterator;
    }

    [CompilerGenerated]
    private sealed class STARTCITERATOR : IEnumerator<object>, IDisposable, IEnumerator
    {
        internal object scurrent;
        internal int SPC;
        internal DownloadTexture FUCKTHIS;
        internal UITexture UT1;
        internal WWW WWW0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.SPC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.SPC;
            this.SPC = -1;
            switch (num)
            {
                case 0:
                    this.WWW0 = new WWW(this.FUCKTHIS.url);
                    this.scurrent = this.WWW0;
                    this.SPC = 1;
                    return true;

                case 1:
                    this.FUCKTHIS.mTex = this.WWW0.texture;
                    if (this.FUCKTHIS.mTex == null)
                    {
                        goto Label_0118;
                    }
                    this.UT1 = this.FUCKTHIS.GetComponent<UITexture>();
                    if (this.UT1.material != null)
                    {
                        this.FUCKTHIS.mMat = new Material(this.UT1.material);
                        break;
                    }
                    this.FUCKTHIS.mMat = new Material(Shader.Find("Unlit/Transparent Colored"));
                    break;

                default:
                    goto Label_012A;
            }
            this.UT1.material = this.FUCKTHIS.mMat;
            this.FUCKTHIS.mMat.mainTexture = this.FUCKTHIS.mTex;
            this.UT1.MakePixelPerfect();
        Label_0118:
            this.WWW0.Dispose();
            this.SPC = -1;
        Label_012A:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.scurrent;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.scurrent;
            }
        }
    }
}

