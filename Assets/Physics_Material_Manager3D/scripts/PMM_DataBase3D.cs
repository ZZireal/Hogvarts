using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FatmindProductions_unity
{
	[System.Serializable]
public class PMM_DataBase3D : ScriptableObject 
	{
		public Vector3 gravity;
		public List<PMM_Variables3D> physicsMaterials3D = new List<PMM_Variables3D>();
	} 
}