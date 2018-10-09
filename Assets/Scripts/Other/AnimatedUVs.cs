using UnityEngine;
using System.Collections;
 
public class AnimatedUVs : MonoBehaviour 
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2( 1.0f, 0.0f );
	public Vector3 uvTargetRate;
    public string textureName = "_MainTex";
 
    Vector2 uvOffset = Vector2.zero;
 	void Start(){
		uvTargetRate = uvAnimationRate;
	}
    void LateUpdate() 
    {
		uvAnimationRate=Vector2.Lerp(uvAnimationRate,uvTargetRate,0.1f);
        uvOffset += ( uvAnimationRate * Time.deltaTime );
        if( GetComponent<Renderer>().enabled )
        {
            GetComponent<Renderer>().materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
        }
    }
}
