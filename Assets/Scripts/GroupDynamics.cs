using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[HideInInspector]
public class GroupDynamics : MonoBehaviour {
	
	public enum GroupModel {
		Box,
		Triangle,
		Circle, 
		Rectangel
	};

	public enum GrouppingMode {
		Strict,
		Tight,
		Loose
	};	

	[Header("Spawns")]
	public List<GameObject> spawnTypes;
	public int spawnCount = 5;

	[HideInInspector]
	public bool randomizeFollowers = true;

	[Header("Dynamics")]
	public GroupModel dynamicModel;

	[HideInInspector]
	public GrouppingMode grouppingMode;

	[HideInInspector]
	public bool followTheLeader;


	[HideInInspector]
	public float height;

	[HideInInspector]
	public float width;


	[HideInInspector]
	private GameObject spawnPlatform;
	[HideInInspector]
	private GameObject followersContainer;
	[HideInInspector]
	private List<GameObject> AI_FOLLOWERS = new List<GameObject>();

	[HideInInspector]
	private Vector3 localBounds;

	[HideInInspector]
	private List<Vector3> usedSpawnLocations = new List<Vector3>();

	// Use this for initialization
	void Start () {

		usedSpawnLocations.Clear();
		usedSpawnLocations.Add(Vector3.zero); 

		//Get my size  
		Bounds bounds = new Bounds();

		foreach (Renderer child in transform.GetComponentsInChildren<Renderer>()){
			bounds.Encapsulate(child.bounds);
		}

		localBounds = bounds.center;
		Debug.Log("Bounds "  + localBounds);
			
		//Create and Align
		spawnPlatform = new GameObject("SpawnPlatform");
		spawnPlatform.transform.position = transform.position;

		followersContainer = new GameObject("Followers Container");
		followersContainer.transform.position = transform.position;
		followersContainer.transform.parent = transform;


		//Draw and size
		spawnPlatform.AddComponent<MeshFilter>();
		MeshFilter meshFilter = spawnPlatform.GetComponent<MeshFilter>();
		AI_FOLLOWERS.Clear();

		if ( dynamicModel == GroupModel.Circle) {
			meshFilter.sharedMesh = MeshHelper.MakeCircle(width);
		} else if (dynamicModel == GroupModel.Box) {
			meshFilter.sharedMesh = MeshHelper.CreateMesh(width, height);				
		} else if (dynamicModel == GroupModel.Rectangel) {
			meshFilter.sharedMesh = MeshHelper.CreateMesh(width, width);				
		} else if (dynamicModel == GroupModel.Triangle) {
			meshFilter.sharedMesh = MeshHelper.MakeTriangel(height, width);				
		}

		while(MeshHelper.PointInMesh(meshFilter.sharedMesh) < spawnCount) {
			MeshHelper.Subdivide9(meshFilter.sharedMesh);
		}

		meshFilter.sharedMesh.Optimize();

		//Color
		spawnPlatform.AddComponent<MeshRenderer>();
		MeshRenderer meshRenderer = spawnPlatform.GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
	

		if (dynamicModel == GroupModel.Rectangel || dynamicModel == GroupModel.Circle) {
			spawnPlatform.transform.parent = transform;
			spawnPlatform.transform.Rotate(new Vector3(-90f,0,0));
			spawnPlatform.transform.localPosition = Vector3.zero;
		} else if (dynamicModel == GroupModel.Box) {
			spawnPlatform.transform.parent = transform;
			spawnPlatform.transform.Rotate(new Vector3(-90f,0,0));
			spawnPlatform.transform.localPosition = new Vector3(0,0,-(height/2));
		}


		Mesh mesh = spawnPlatform.GetComponent<MeshFilter>().sharedMesh;

		for(int i = 0; i < spawnCount; i++){
			
			Vector3 spawnPoint = MeshHelper.GetRandomLocation(mesh);

			while(usedSpawnLocations.Contains(spawnPoint)) {
				spawnPoint = MeshHelper.GetRandomLocation(mesh);
			}

			usedSpawnLocations.Add(spawnPoint);	
			GameObject aiChild = (GameObject) Instantiate (spawnTypes[0], spawnPoint, Quaternion.identity);
			aiChild.transform.parent = followersContainer.transform;
			Vector3 currentPosition = aiChild.transform.position;

			if (dynamicModel != GroupModel.Triangle) {
				currentPosition = Quaternion.Euler(-90, 0,0) * currentPosition;
			}

			if ( dynamicModel == GroupModel.Box) {
				currentPosition.z -= (height/2);
			}

			aiChild.transform.localPosition = transform.TransformDirection(currentPosition);
			AI_FOLLOWERS.Add(aiChild);
		}

		usedSpawnLocations.Clear();
		Destroy(spawnPlatform);

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		if (dynamicModel == GroupDynamics.GroupModel.Triangle) {
			Vector3 leftSide =  new Vector3(transform.position.x - (width /2), transform.position.y, transform.position.z - height);
			Vector3 rightSide =  new Vector3(transform.position.x + (width /2), transform.position.y, transform.position.z - height);

			Gizmos.DrawLine(transform.position, leftSide);
			Gizmos.DrawLine(transform.position, rightSide);
			Gizmos.DrawLine(leftSide, rightSide);

		} else if ( dynamicModel == GroupModel.Circle) {
			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc(transform.position , transform.up, width);




		} else if ( dynamicModel == GroupModel.Box) {
			Vector3 leftButtom =  new Vector3(transform.position.x - (width /2), transform.position.y, transform.position.z - height);
			Vector3 rightButtom =  new Vector3(transform.position.x + (width /2), transform.position.y, transform.position.z - height);

			Vector3 leftTop =  new Vector3(transform.position.x - (width /2), transform.position.y, transform.position.z);
			Vector3 rightTop =  new Vector3(transform.position.x + (width /2), transform.position.y, transform.position.z);

			Gizmos.DrawLine(leftTop, leftButtom);
			Gizmos.DrawLine(leftButtom, rightButtom);
			Gizmos.DrawLine(rightButtom, rightTop);
			Gizmos.DrawLine(rightTop, leftTop);

		} else if (dynamicModel == GroupModel.Rectangel) {
			
			Vector3 leftButtom =  new Vector3(transform.position.x - (width /2), transform.position.y, transform.position.z - (width/2));
			Vector3 rightButtom =  new Vector3(transform.position.x + (width /2), transform.position.y, transform.position.z - (width/2));

			Vector3 leftTop =  new Vector3(transform.position.x - (width /2), transform.position.y, transform.position.z + (width/2));
			Vector3 rightTop =  new Vector3(transform.position.x + (width /2), transform.position.y, transform.position.z + (width/2));

			Gizmos.DrawLine(leftTop, leftButtom);
			Gizmos.DrawLine(leftButtom, rightButtom);
			Gizmos.DrawLine(rightButtom, rightTop);
			Gizmos.DrawLine(rightTop, leftTop);

		}

	}


}
