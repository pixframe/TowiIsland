/* ===============================================================
 * Company   : Digitech Gamez
 *
 * Module    : JointManager.cs (Unity 3.5)
 * 
 * Desc      : Attach this as a component to the root asset.
 * 
 *             Reference the joints of a child mesh 
 *             to the joints of its parent mesh 
 *             It achieves this by assigning each bone
 *             transform of the child mesh to the corresponding
 *             transform of the parent mesh bone, such
 *             that the bones moves together.
 * 
 * Author    : gxmark
 * 
 * Date      : April 2012
 * 
 * Copyright : Royalty Free
 * ===============================================================
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class JointManager : MonoBehaviour 
{
	// References to SkinnedMeshRenderer
	private SkinnedMeshRenderer parent_smr;
	private Dictionary<string,SkinnedMeshRenderer>child_smr_map = new Dictionary<string,SkinnedMeshRenderer>();
	Hashtable originalRef;
	
	void Start () 
	{

	}
	
	public void Initialize(GameObject rootObj)
	{
		if (rootObj != null)
		{
			originalRef = new Hashtable ();
			// Get the SkinnedMeshRenderer of the parent mesh
			SkinnedMeshRenderer[] skinned=(SkinnedMeshRenderer[])rootObj.GetComponentsInChildren<SkinnedMeshRenderer>();
			parent_smr = skinned[skinned.Length-1];

			Transform[] parent_bones = parent_smr.bones;
			foreach(Transform parent_bone in parent_bones)
			{
				originalRef.Add(parent_bone.name,new JointOriginal(parent_bone.position,parent_bone.rotation));
			}
		}
	}
	
	// Attach the mesh joints to the parent joints
	public void AttachToParent(GameObject meshObj)
	{
		if (meshObj != null)
		{
			//WWW.fridaysvip.mx
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] objRenderers = meshObj.GetComponentsInChildren<SkinnedMeshRenderer>();
			for(int i=0;i<renderers.Length;i++)
			{
				renderers[i].enabled=false;
			}
			for(int i=0;i<objRenderers.Length;i++)
			{
				objRenderers[i].enabled=false;
			}
			//renderer.enabled=false;
			//rigidbody.useGravity=false;
			SkinnedMeshRenderer[] smr = meshObj.GetComponentsInChildren<SkinnedMeshRenderer>();

			//child_smr_map.Add(meshObj.name,smr[0]);
			Animator anim=GetComponent<Animator>();
			anim.enabled=false;
			//transform.position=new Vector3(8,0,0);
			// Get the meshes bones
			List<Transform> tempBones=new List<Transform>();
			for(int i=0;i<smr.Length;i++)
			{
				for(int idx=0;idx<smr[i].bones.Length;idx++)
				{
					bool found=false;
					for(int x=0;x<tempBones.Count;x++)
					{
						if(tempBones[x].name==smr[i].bones[idx].name)
						{
							found=true;
							break;
						}
					}
					if(!found)
					{
						tempBones.Add(smr[i].bones[idx]);
					}
				}
			}
			Transform[] mesh_bones = tempBones.ToArray();
			
			// Get the parent mesh (character) bones
			Transform[] parent_bones = parent_smr.bones;

			foreach(Transform parent_bone in parent_bones)
			{
				JointOriginal tempOrig=originalRef[parent_bone.name] as JointOriginal;
				parent_bone.position=tempOrig.position;
				parent_bone.rotation=tempOrig.rotation;
			}

			foreach(Transform parent_bone in parent_bones)
			{
				foreach(Transform mesh_bone in mesh_bones)
					if (parent_bone.name.Contains(mesh_bone.name))
					{
						// Assign the mesh_bone to the parent bone transform
						mesh_bone.position=parent_bone.position;
						//mesh_bone.rotation = parent_bone.rotation;
						mesh_bone.parent = parent_bone;
						
						//parent_bone.position=tempPos;
						//parent_bone.rotation=tempRot;
					    print ("mesh bone parent = " + mesh_bone.parent.name + " parent bone = " + parent_bone.name);
					    break;
					}
			}
			anim.enabled=true;
			for(int i=0;i<renderers.Length;i++)
			{
				renderers[i].enabled=true;
			}
		}
	}
	
	// Detach the mesh joints from the parent joints
	public void DetachFromParent(GameObject meshObj)
	{
		if (meshObj != null)
		{
			// Get the meshes bones
			SkinnedMeshRenderer smr = child_smr_map[meshObj.name];
			
			
			foreach(Transform mesh_bone in smr.bones)
				mesh_bone.parent = smr.transform;
			
			// Remove the mesh from the child smr map
			child_smr_map.Remove(meshObj.name);
			
			GameObject.Destroy(smr);
		}		
	}

	public class JointOriginal
	{
		public Vector3 position;
		public Quaternion rotation;
		public JointOriginal(Vector3 pos, Quaternion rot)
		{
			position=pos;
			rotation=rot;
		}
	}
}

