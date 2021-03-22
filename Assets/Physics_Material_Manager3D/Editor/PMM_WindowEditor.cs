using UnityEngine;
using System.Collections;
using UnityEditor;

namespace FatmindProductions_unity
{
	[CanEditMultipleObjects]
	public class PMM_WindowEditor : EditorWindow
	{
		PMM_DataBase3D db3D;
		Vector2 scrollPos; 
		Texture2D physicsIcon;
		GUISkin skin;

		Rect headersection = new Rect();

		[MenuItem("Window/Physics Material Manager3D")]
		public static void Init()
		{
			PMM_WindowEditor window = EditorWindow.GetWindow<PMM_WindowEditor>();
			window.minSize =  new Vector2 (385,400);
			window.titleContent.text = "Phys. Manager";
			window.Show();
		}
	
		public void OnEnable()
		{
			LoadDataBase();
			DrawHeaderSection();
			DrawHeaderIcon();
			skin = Resources.Load<GUISkin>("guiStyles/PMM_skin");
		}

		public  void CreateDataBase3D()
		{   
			db3D = ScriptableObject.CreateInstance<PMM_DataBase3D>();
		
			AssetDatabase.FindAssets("Assets/Physics_Material_Manager3D/AssetDataBase/database3D.asset");
			AssetDatabase.CreateAsset(db3D, "Assets/Physics_Material_Manager3D/AssetDataBase/database3D.asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}	

		public void DrawHeaderSection()
		{
			headersection.x = 10;
			headersection.y = 20;
			headersection.width = 40;
			headersection.height = 40;
		}

		public void DrawHeaderIcon()
		{
			physicsIcon = new Texture2D(1,1);
			physicsIcon.SetPixel(0,0,Color.grey);
			physicsIcon.Apply();
			physicsIcon = Resources.Load<Texture2D>("icons/physics_icon");
		}

		void LoadDataBase()
		{
			db3D = (PMM_DataBase3D)AssetDatabase.LoadAssetAtPath("Assets/Physics_Material_Manager3D/AssetDataBase/database3D.asset", typeof(PMM_DataBase3D));


			if(db3D == null)
				{
					CreateDataBase3D();
				}
		}
	
		public void OnGUI()
		{
		scrollPos = GUILayout.BeginScrollView(scrollPos,false,false);
		GUILayout.Space(15);
		
			GUILayout.BeginVertical("Box");//outline box

			GUILayout.BeginHorizontal();

				GUI.DrawTexture(headersection,physicsIcon);
				GUILayout.Space(50);
				EditorGUILayout.LabelField("Physics Material Manager3D", skin.GetStyle("Header"),GUILayout.MaxWidth(450),GUILayout.MinWidth(200));

			GUILayout.EndHorizontal();

				GUILayout.Space(30);

			GUILayout.BeginVertical("Box");//box around World Gravity Setting
					GUILayout.Space(5);
			GUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("World Gravity Setting",skin.GetStyle("PM_Bold"),GUILayout.MaxWidth(120));
				db3D.gravity = EditorGUILayout.Vector3Field(GUIContent.none,db3D.gravity,GUILayout.MinWidth(200));
				Physics.gravity = db3D.gravity;

			GUILayout.EndHorizontal();

				GUILayout.Space(5);

			GUILayout.EndVertical(); //box around World Gravity Setting

				GUILayout.Space(5);


			
			for (int count = 0; count < db3D.physicsMaterials3D.Count; count++) 
			{
								
									GUILayout.Space(10);
								GUILayout.BeginHorizontal();

								EditorGUILayout.LabelField("Physics Material 3D",skin.GetStyle("PM_Bold"),GUILayout.MaxWidth(120));

								GUILayout.Space(10);

									db3D.physicsMaterials3D[count].physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(GUIContent.none,db3D.physicsMaterials3D[count].physicMaterial,typeof(PhysicMaterial),false
									,GUILayout.MinWidth(150),GUILayout.MaxWidth(150));

								GUILayout.Space(9);

											if (GUILayout.Button("Remove",GUILayout.MaxWidth(55),GUILayout.MinWidth(70)))
											{
												RemovePhysicsMaterial3D(count);
												return;
											}

								GUILayout.EndHorizontal();

									GUILayout.Space(5);

									if(db3D.physicsMaterials3D[count].physicMaterial == null){}
										else 
										{
										GUILayout.BeginHorizontal();
											EditorGUILayout.LabelField(new GUIContent("Dynamic Friction", "A value of zero feels like ice, a value of 1 will make it come to a rest very quickly")
													,skin.GetStyle("PM_Bold"),GUILayout.MinWidth(50));
														db3D.physicsMaterials3D[count].dynamicFriction = EditorGUILayout.Slider(db3D.physicsMaterials3D[count].dynamicFriction,0,1
														,GUILayout.MinWidth(50),GUILayout.MaxWidth(200));

										GUILayout.EndHorizontal();
												db3D.physicsMaterials3D[count].physicMaterial.dynamicFriction = db3D.physicsMaterials3D[count].dynamicFriction;

										GUILayout.BeginHorizontal();
											EditorGUILayout.LabelField(new GUIContent("Static Friction", "A value of zero feels like ice, a value of 1 will make it very hard to get the object to moving")
													,skin.GetStyle("PM_Bold"),GUILayout.MinWidth(50));
														db3D.physicsMaterials3D[count].staticFriction = EditorGUILayout.Slider(db3D.physicsMaterials3D[count].staticFriction,0,1
														,GUILayout.MinWidth(50),GUILayout.MaxWidth(200));

										GUILayout.EndHorizontal();
												db3D.physicsMaterials3D[count].physicMaterial.staticFriction = db3D.physicsMaterials3D[count].staticFriction;

										GUILayout.BeginHorizontal();
											EditorGUILayout.LabelField(new GUIContent("Bounceiness", "A value of zero will not bouce. A value of 1 will bounce without any loss of energy")
													,skin.GetStyle("PM_Bold"),GUILayout.MinWidth(50));
														db3D.physicsMaterials3D[count].bounciness = EditorGUILayout.Slider(db3D.physicsMaterials3D[count].bounciness,0,1
														,GUILayout.MinWidth(50),GUILayout.MaxWidth(200));

										GUILayout.EndHorizontal();
											db3D.physicsMaterials3D[count].physicMaterial.bounciness = db3D.physicsMaterials3D[count].bounciness;

										GUILayout.BeginHorizontal();
											EditorGUILayout.LabelField(new GUIContent("Friction Combine", "How the friction of two collidong object is combined"),skin.GetStyle("PM_Bold"));
												db3D.physicsMaterials3D[count].frictionCombine = (PhysicMaterialCombine)EditorGUILayout.EnumPopup(db3D.physicsMaterials3D[count].frictionCombine
												,GUILayout.MaxWidth(200));

										GUILayout.EndHorizontal();
											db3D.physicsMaterials3D[count].physicMaterial.frictionCombine = db3D.physicsMaterials3D[count].frictionCombine;

										GUILayout.BeginHorizontal();
												EditorGUILayout.LabelField(new GUIContent("Bounce Combine", "How the bounciness of two collidong object is combined"),skin.GetStyle("PM_Bold"));
												db3D.physicsMaterials3D[count].bounceCombine = (PhysicMaterialCombine)EditorGUILayout.EnumPopup(db3D.physicsMaterials3D[count].bounceCombine
												,GUILayout.MaxWidth(200));

										GUILayout.EndHorizontal();
											db3D.physicsMaterials3D[count].physicMaterial.bounceCombine = db3D.physicsMaterials3D[count].bounceCombine;
										}//end of else loop 
					GUILayout.Space(15);
			
			}//End of for loop	

					if(GUILayout.Button("Add Physics Material",GUILayout.MaxWidth(370)))
						{
						AddPhysicsMaterial3D();
						}

					GUILayout.Space(5);

			EditorUtility.SetDirty(db3D);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}//for of onGUI

		void AddPhysicsMaterial3D()
			{
				db3D.physicsMaterials3D.Add( new PMM_Variables3D());
			}

		void RemovePhysicsMaterial3D(int index )
			{
				db3D.physicsMaterials3D.RemoveAt(index);
			}
	}
}