using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class Pixelation : MonoBehaviour {

    private Material mat;
    [Range(32, 800), Header("Pixelation")]
    public int resolution = 400;
    [Header("Dithering")]
    public Texture2D ditheringMatrix;
    public int ditheringMatrixSize = 16;
    [Header("Color Reduction")]
    public Texture3D lut;
    public enum ColorRemappingMode{
        Reduce,
        LUT
    }
    public ColorRemappingMode colorRemappingMode = ColorRemappingMode.Reduce;
    public float colorSpread = 0.125f;
    public int numColors = 16;
    public enum ColorReductionMode{
        Pre,
        Post
    };
    public ColorReductionMode colorReductionMode = ColorReductionMode.Post;
    [Header("Range Remapping")]
    [Range(0.0f, 1.0f)]
    public float lowerBound = 0.0f;
    [Range(0.0f, 1.0f)]
    public float upperBound = 1.0f;

	private void Awake () {
        mat = new Material(Shader.Find("Hidden/Pixelation"));
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(mat == null)
            mat = new Material(Shader.Find("Hidden/Pixelation"));
        mat.SetInt("_Pixels", resolution);
        mat.SetTexture("_DitheringMatrix", ditheringMatrix);
        mat.SetInt("_DitheringMatrixSize", ditheringMatrixSize);
        if(colorRemappingMode == ColorRemappingMode.Reduce){
            mat.DisableKeyword("COLORREMAPMODE_LUT");
            mat.EnableKeyword("COLORREMAPMODE_REDUCE");
        }
        else{
            mat.DisableKeyword("COLORREMAPMODE_REDUCE");
            mat.EnableKeyword("COLORREMAPMODE_LUT");
            mat.SetTexture("_LUT", lut);
        }
        mat.SetFloat("_ColorSpread", colorSpread);
        mat.SetInt("_NumColors", numColors);
        if(colorReductionMode == ColorReductionMode.Pre){
            mat.DisableKeyword("COLORREDUCTIONMODE_POST");
            mat.EnableKeyword("COLORREDUCTIONMODE_PRE");
        }
        else{
            mat.DisableKeyword("COLORREDUCTIONMODE_PRE");
            mat.EnableKeyword("COLORREDUCTIONMODE_POST");
        }
        mat.SetFloat("_LowerBound", lowerBound);
        mat.SetFloat("_UpperBound", upperBound);
        source.filterMode = FilterMode.Point;
        Graphics.Blit(source, destination, mat);
    }

}
