using UnityEngine;
using System.Collections;

namespace FatmindProductions_unity
{
[System.Serializable]
public class PMM_Variables3D
	{
		public PhysicMaterial physicMaterial;
		public float dynamicFriction;
		public float staticFriction;
		public float bounciness;
		public PhysicMaterialCombine frictionCombine;
		public PhysicMaterialCombine bounceCombine;
	}
}