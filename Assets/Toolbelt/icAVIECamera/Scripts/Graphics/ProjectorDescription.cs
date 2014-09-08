using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Toolbelt;

namespace Assets.AVIE.Scripts.Graphics
{
	
    public class ProjectorDescription
    {

		public const string CONFIG_PATH = "D:/UnityConfig/";
        public enum ProjectorStereoType
        {
            Left,
            Right
        };
		
        public int nodeNumber;
        private ProjectorMesh projectorMesh;
        public ProjectorStereoType stereoType;
        public Vector3[] meshVertices;
        public Vector2[] meshUVs;
        public int[] triangles;
        public RenderTexture _targetRenderTexture;
		public Texture2D _targetBlendTexture;
        public Mesh mesh;
        public GameObject warpMeshObject;
        //private List<Vector2> cylinderVertices;
        //private List<Vector2> cylinderUVs;
        public List<SliceCameraDescription> sliceCameras = new List<SliceCameraDescription>();
        public GameObject _orthographicCamera;
        private GameObject _projectorObject;

        float maxMeshX, maxMeshY, minMeshX, minMeshY;
		public bool imageShaderScriptFirstEnabled = false;
        public ProjectorMesh ProjectorMesh
        {
            get
            {
                return projectorMesh;
            }
        }

        public GameObject ProjectorObject
        {
            get
            {
                return _projectorObject;
            }
        }

        /**
         * Ctor.
         * @param id The projector id.
         * @param width width of the projector grid.
         * @param height height of the projector grid.
         * @param start start seam for the projector.
         * @param end end seam for the projector.
         */
        public ProjectorDescription(ProjectorMesh projectorMesh, 
            int renderTextureWidth,
            int renderTextureHeight,
            GameObject warpMeshPrefab,
            Transform clusterManager)
        {
            this.projectorMesh = projectorMesh;
            nodeNumber = projectorMesh.Id / 2;

            if (projectorMesh.Id % 2 == 0)
            {
                stereoType = ProjectorStereoType.Left;
            }
            else if (projectorMesh.Id % 2 == 1)
            {
                stereoType = ProjectorStereoType.Right;
            }
            meshVertices = new Vector3[projectorMesh.NumVertices];
            meshUVs = new Vector2[projectorMesh.NumVertices];
            triangles = new int[3 * 2 * projectorMesh.GridWidth * projectorMesh.GridHeight];

            _projectorObject = new GameObject("Ortho Camera Rig: " + projectorMesh.Id);            

            //
            // Position this rig under the Cluster Manager 
            // to make the scene browser clearer
            _projectorObject.transform.parent = clusterManager;

            /////////////////
            // Create Mesh //
            /////////////////
            Mesh mesh = this.CreateMesh();
            if (mesh == null)
            {
                // Create mesh failed, exit
                throw new Exception("Mesh Creation Failed.");
            }
//            Debug.Log("Mesh Created.");

            // Projector is the left of the two assigned to this IG
            string warpMeshPrefix = "IG(" + (projectorMesh.Id / 2) + ").Projector(" + projectorMesh.Id + ")";            
            GameObject warpMeshObject = GameObject.Instantiate(warpMeshPrefab) as GameObject;
            warpMeshObject.name = warpMeshPrefix + ".object";
            //GameObject warpMeshObject = new GameObject(warpMeshPrefix + ".object");            
            warpMeshObject.transform.parent = this.ProjectorObject.transform;            
            //warpMeshObject.AddComponent("MeshFilter");
            //warpMeshObject.AddComponent("MeshRenderer");
            (warpMeshObject.GetComponent("MeshFilter") as MeshFilter).mesh = mesh;
            MeshRenderer meshRenderer = warpMeshObject.GetComponent<MeshRenderer>();
            meshRenderer.castShadows = false;
            meshRenderer.receiveShadows = false; 

            this.warpMeshObject = warpMeshObject;
            
            
            ///////////////////////////////////////////////////
            // Create a new render texture for the projector //
            ///////////////////////////////////////////////////
            int renderTextureDepth = 24;            
            RenderTexture renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, renderTextureDepth);
            this._targetRenderTexture = renderTexture;
			
			///////////////////////////////////////////////////
			// Create a new blend texture for this projector //
			///////////////////////////////////////////////////
			string path = "file:///" + CONFIG_PATH + "Avie/BlendTextures/blend" + projectorMesh.Id.ToString("00") + ".png";
			WWW www = new WWW(path);
			this._targetBlendTexture = www.texture;

			if (www.error != null)
				Debug.Log (www.error);

			///////////////////////////////////////////////////
			// Add HSV/ Gamma adjust						 //        
			///////////////////////////////////////////////////
			icMaterial icm = this.warpMeshObject.AddComponent<icMaterial>();
			icm.multiplyByParentColour = false;
			
			
			RefreshTextureScript rts = this.warpMeshObject.AddComponent<RefreshTextureScript>();
			rts.toReplace = this._targetRenderTexture;
			ImageShaderScript iss = this.warpMeshObject.AddComponent<ImageShaderScript>();
			
			iss.separatePass = true;         
			iss.enabled = false;   
			//imageShaderScript needs to be enabled one frame later, or a white flash occurs...
									
        }

        // This method cannot be called from the destructor, as the
        // destructor is called from the loading thread, whereas
        // GameObject.Destory() can only be called from the main thread
        public void destroyGameObjects()
        {
            GameObject.Destroy(_projectorObject);
            GameObject.Destroy(_targetRenderTexture);
        }

        ~ProjectorDescription()
        {
            //Debug.Log("Destroying ProjectorDescription");            
        }

        // Create one individual mesh and assign it to its correct render plane
        public Mesh CreateMesh()
        {
            //cylinderVertices = new List<Vector2>();
            //cylinderUVs = new List<Vector2>();
            int numVertices = projectorMesh.NumVertices;
//            float meshStart = projectorMesh.MeshStart;
//            float meshEnd = projectorMesh.MeshEnd;
            int gridHeight = projectorMesh.GridHeight;
            int gridWidth = projectorMesh.GridWidth;

            maxMeshX = float.NegativeInfinity;
            maxMeshY = float.NegativeInfinity;
            minMeshX = float.PositiveInfinity;
            minMeshY = float.PositiveInfinity;

            for (int i = 0; i < numVertices; i++)
            {
                ProjectorMesh.CylinderPoint point = projectorMesh.GetCylinderPoint(i);

                // cylinderX is between -0.5f and 0.5f where
                // -0.5f is projector start and 0.5f is projector end

                // projectorX should go from projectorStart to projectorEnd;
                // cylinderX gives the angle in cylindrical coordinates.
                meshVertices[i].x = point.xy.x;
                meshVertices[i].y = point.xy.y;
                meshVertices[i].z = 0;
                meshUVs[i].x = point.uv.x;
                meshUVs[i].y = 1.0f - point.uv.y;

                if (point.xy.x > maxMeshX)
                {
                    maxMeshX = point.xy.x;
                }
                if (point.xy.y > maxMeshY)
                {
                    maxMeshY = point.xy.y;
                }
                if (point.xy.x < minMeshX)
                {
                    minMeshX = point.xy.x;
                }
                if (point.xy.y < minMeshY)
                {
                    minMeshY = point.xy.y;
                }

            }

            // Create triangles
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int tri = (y * gridWidth + x) * 6;

                    triangles[tri + 0] = y * (gridWidth + 1) + x;
                    triangles[tri + 1] = triangles[tri + 0] + 1;
                    triangles[tri + 2] = triangles[tri + 1] + gridWidth;

                    triangles[tri + 3] = triangles[tri + 2];
                    triangles[tri + 4] = triangles[tri + 1];
                    triangles[tri + 5] = triangles[tri + 2] + 1;
                }
            }

            string warpMeshPrefix = "IG(" + nodeNumber + ").Projector(" + projectorMesh.Id + ")";

            // Create mesh from vertices and triangles
            mesh = new Mesh();
            mesh.name = warpMeshPrefix + ".mesh";
            mesh.vertices = meshVertices;
            mesh.uv = meshUVs;
            mesh.triangles = triangles;

            return mesh;
        }

		public List<SliceCameraDescription> CreateSliceCameras(float[] sliceSeams, GameObject parent, 
																StereoCameraInterface stereoCameraInterface, 
																Camera copySliceCameraFrom)
		{            
            int numSlices; // number of slices visible to this projector
            int firstSeam = -1;
            int lastSeam = -1;
            float projStart = projectorMesh.MeshStart % 1.0f;
            float projEnd = projectorMesh.MeshEnd % 1.0f;
//            float projWidth = (projectorMesh.MeshStart > projectorMesh.MeshEnd)
//            ? (1.0f + projectorMesh.MeshEnd - projectorMesh.MeshStart)
//            : (projectorMesh.MeshEnd - projectorMesh.MeshStart); 
            float[] contextSeams = new float[sliceSeams.Length]; // create a local version of slice seams that may be modified

            sliceSeams.CopyTo(contextSeams, 0);

            //Debug.Log("Projector Start: " + projStart.ToString() + ". Projector End: " + projEnd.ToString());

            for (int i = 0; i < contextSeams.Length; i++)
            {
                float testLeftSeam = contextSeams[i];
                // The 0.0 seam must also act as 1.0 if it is at the end.
                float testRightSeam = ((i + 1) == contextSeams.Length) ? 1.0f : contextSeams[i + 1];
                if (testLeftSeam <= projStart && testRightSeam > projStart)
                {
                    // Left Projector edge is between these two seams
                    firstSeam = i;
                    //Debug.Log("Found first seam: " + contextSeams[i].ToString());
                }
                else if (testLeftSeam < projEnd && testRightSeam >= projEnd)
                {
                    // Right Projector edge is between these two seams
                    lastSeam = (i + 1) % contextSeams.Length;
                    //Debug.Log("Found last seam: " + contextSeams[(i + 1) % contextSeams.Length].ToString());
                }
            }

            if (firstSeam == -1 || lastSeam == -1)
            {
                // For some reason, couldn't find seams
                throw new Exception("Couldn't find slice seams on either side of the projector");
            }
            if (lastSeam > firstSeam)
            {
                numSlices = (lastSeam - firstSeam) % contextSeams.Length;
            }
            else
            {
                // this projector wraps around the 0 seam
                //Debug.Log("This Projector touches the 0 seam, fixing values");
                numSlices = (lastSeam + (contextSeams.Length - firstSeam));

                for (int i = 0; i < contextSeams.Length; i++)
                {
                    if (i >= 0 && i <= lastSeam)
                    {
                        contextSeams[i] += 1.0f;
                    }
                }

                if (projEnd < projStart)
                {
                    projEnd += 1.0f;
                }
            }

            // Make a camera per slice (clone the main camera's position and orientation)
            for (int i = 0; i < numSlices; i++)
            {
                float leftSeam = contextSeams[(firstSeam + i) % contextSeams.Length];
                float rightSeam = contextSeams[(firstSeam + i + 1) % contextSeams.Length];
                float decAngle = (rightSeam < leftSeam) ? (leftSeam + rightSeam + 1.0f) / 2.0f : (leftSeam + rightSeam) / 2.0f;
                float angle = decAngle * 360.0f;
				SliceCameraDescription sliceCamera = new SliceCameraDescription(i, angle, leftSeam, rightSeam, this, parent, copySliceCameraFrom);
                sliceCameras.Add(sliceCamera);
                
                // Add eye callback script to slice camera
                CameraEyeCallback callbackScript = sliceCamera.cameraObject.AddComponent<CameraEyeCallback>();
				callbackScript.stereoCameraInterface = stereoCameraInterface;
				StereoMode stereoMode = this.stereoType == ProjectorStereoType.Left ? StereoMode.LEFT : StereoMode.RIGHT;
				callbackScript.stereoMode = stereoMode;
				
				Camera camera = sliceCamera.cameraObject.camera;
				
				StereoMode oppositeStereo = stereoMode == StereoMode.LEFT ? StereoMode.RIGHT : StereoMode.LEFT;
				camera.cullingMask = camera.cullingMask & ~(1 << ToolbeltManager.FirstInstance.GetStereoLayer(oppositeStereo));
            }
            return sliceCameras;
        }

        public void CreateDisplayCamera(GameObject parent, Camera sourceCamera)
        {
//            Mesh mesh = warpMeshObject.GetComponent<MeshFilter>().mesh;
            _orthographicCamera = new GameObject("Slave Camera" + projectorMesh.Id);
            _orthographicCamera.AddComponent("Camera");
            _orthographicCamera.camera.CopyFrom(sourceCamera);
            _orthographicCamera.camera.targetTexture = null;
            _orthographicCamera.camera.backgroundColor = Color.black;
            _orthographicCamera.transform.parent = _projectorObject.transform;
            
            // These parameters should already be set in source camera.
            //_orthoCameraTemplate.camera.orthographic = true;
            _orthographicCamera.camera.projectionMatrix = OrthoMatrix(-0.5f, 0.5f, -0.5f, 0.5f, 0.3f, 1000.0f);

            Vector3 v = new Vector3(0,0,-1);
            _orthographicCamera.transform.position = v;
            _orthographicCamera.transform.LookAt(Vector3.zero);

            _projectorObject.transform.Translate(new Vector3(5.0f * projectorMesh.Id, 1000, 1000));
        }


        public static Matrix4x4 OrthoMatrix(float left, float right, float bottom, float top, float near, float far)
        {
            float x = 2.0f / (right - left);
            float y = 2.0f / (top - bottom);
            float z = -2.0f / (far - near);
            float a = -(right + left) / (right - left);
            float b = -(top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = 1.0f;
			
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = 0;
            m[0, 3] = a;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = 0;
            m[1, 3] = b;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = z;
            m[2, 3] = c;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = 0;
            m[3, 3] = d;
            return m;

        }
    }

   


}