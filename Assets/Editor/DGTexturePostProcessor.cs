/* =================================================================
 * Company   : Digitech Gamez
 *
 * Module    : DGTexturePostprocessor.cs (Unity 3.5)
 * 
 * Desc      : Editor Script
 *             Intercepts textures on import containing _bumpmap and 
 *             converts them to normal maps
 * 
 * Author    : gxmark
 * 
 * Date      : April 2012
 * 
 * Copyright : Royalty Free
 * =================================================================
 */

using UnityEngine;
using UnityEditor;


public class DGTexturePostProcessor : AssetPostprocessor 
{
    void OnPreprocessTexture () 
    {
		// Detect bump texture and convert to normal map
        if (assetPath.Contains("bump_"))
		{
	    	TextureImporter importer = assetImporter as TextureImporter;
			importer.textureType = TextureImporterType.NormalMap;
			importer.heightmapScale = 0.02f;
	    	importer.convertToNormalmap = true;
        }
		
		// Detect transparency texture and convert to use alpha scale
		if (assetPath.Contains("tran_"))
		{
	    	TextureImporter importer = assetImporter as TextureImporter;
			importer.grayscaleToAlpha = true;
        }
    }
}



