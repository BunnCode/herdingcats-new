using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that controls the multibillboard shader.
/// todo: Currently hacked to only work for cats, but will be updated.
/// </summary>
public class MultiBillboard : MonoBehaviour {
    /// <summary>
    /// Textures to be used for the multi-billboarding, frame 1
    /// </summary>
    public Texture2D[] Textures;

    /// <summary>
    /// Textures to be used for the multi-billboarding, frame 2
    /// todo: fix this hack into something more elegant
    /// </summary>
    public Texture2D[] TexturesFrame2;

    //todoThis being static is a temporary hotfix for the alpha build and will be rectified
    private static Texture2D _generatedTexture;

    private Texture2D GetGeneratedTexture() {
        if (!_generatedTexture) {
            int textureWidth = Textures[0].width;
            int textureHeight = Textures[0].height;

            if (!_generatedTexture) {
                _generatedTexture = new Texture2D(textureWidth * Textures.Length, textureHeight * 2);

                //Iterate over all frames
                for (int i = 0; i < Textures.Length; i++) {
                    //Frame 1
                    _generatedTexture.SetPixels(
                        (i * textureWidth),
                        0,
                        Textures[i].width,
                        Textures[i].height,
                        Textures[i].GetPixels());
                    //Frame 2
                    _generatedTexture.SetPixels(
                        (i * textureWidth),
                        Textures[i].height,
                        TexturesFrame2[i].width,
                        TexturesFrame2[i].height,
                        TexturesFrame2[i].GetPixels());
                }
#if UNITY_EDITOR
                _generatedTexture.alphaIsTransparency = true;
#endif
                _generatedTexture.Apply();
            }
        }

        return _generatedTexture;
    }

    private Material _material;
    private Renderer _renderer;

    public MaterialPropertyBlock block { get; private set; }

    // Start is called before the first frame update
    void Start() {
        _material = GetComponent<Material>();
        _renderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        //Generate texture- temporary hack for the alpha build
        GetGeneratedTexture();
    }

    // Update is called once per frame
    void LateUpdate() {
        Matrix4x4 trMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, transform.lossyScale);
        block.SetTexture("_MainTex", GetGeneratedTexture());
        block.SetMatrix("_TS_Matrix", trMatrix);
        _renderer.SetPropertyBlock(block);
    }
}