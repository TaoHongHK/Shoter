using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MotionBlur : MonoBehaviour
{
    public Shader m_motionBlurShader;

    private Material _motionBlurMaterial;
    private Material MotionBlurMaterial
    {
        get
        {
            if (_motionBlurMaterial == null)
            {
                _motionBlurMaterial = new Material(m_motionBlurShader);
            }
            return _motionBlurMaterial;
        }
    }

    [Range(0, 0.9f)]
    public float m_blurAmount = 0.5f;

    public RenderTexture _accumulationTexture;

    private void OnEnable()
    {
        _accumulationTexture = null;
    }

    private void OnDisable()
    {
        DestroyImmediate(_accumulationTexture);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (MotionBlurMaterial != null)
        {
            // Create the accumulation texture
            if (_accumulationTexture == null ||
                _accumulationTexture.width != source.width ||
                _accumulationTexture.height != source.height)
            {
                DestroyImmediate(_accumulationTexture);
                _accumulationTexture = new RenderTexture(source.width, source.height, 0);
                _accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, _accumulationTexture);
            }

            _accumulationTexture.MarkRestoreExpected();

            MotionBlurMaterial.SetFloat("_BlurAmount", 1.0f - m_blurAmount);

            Graphics.Blit(source, _accumulationTexture, MotionBlurMaterial);
            Graphics.Blit(_accumulationTexture, destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
