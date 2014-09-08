using System;
using UnityEngine;
namespace Assets.AVIE.Scripts.Graphics
{
    public class SliceCameraDescription
    {
        private int id;

        //Camera mainCamera;
       
        public Rect originalCameraRect { get; set; }
        private float angleDegrees;
        public GameObject cameraObject { get; set; }
        private float leftSeam;
        private float rightSeam;
        private ProjectorDescription projector;
        private string sliceName;
        private float cameraAngle;
        private float renderRadius;
        private float renderHeight;
        private float eyeWidth;
        private float eyeHeight;
        private float sliceWidth;

        public int Id
        {
            get
            {
                return id;
            }
        }

        public float Angle
        {
            get
            {
                return angleDegrees;
            }
        }

        public GameObject CameraObject
        {
            get
            {
                return cameraObject;
            }

            set
            {
                cameraObject = value;
                this.originalCameraRect = new Rect(cameraObject.camera.rect);
            }
        }

        public float EyeWidth
        {
            get
            {
                return eyeWidth;
            }

            set
            {
                float previousValue = eyeWidth;
                eyeWidth = value;
                if (eyeWidth != previousValue)
                {
                    ComputeProjection(cameraAngle, renderRadius, renderHeight, eyeWidth, eyeHeight, sliceWidth);
                }
            }
        }
        
        public ProjectorDescription Projector {
        	get { return this.projector; }
        }

		public SliceCameraDescription(int id, float angle, float leftSeam, float rightSeam, ProjectorDescription projector, GameObject parent, Camera copyFrom)
        {
            this.id = id;
            this.angleDegrees = angle;
            this.leftSeam = leftSeam;
            this.rightSeam = rightSeam;
            this.projector = projector;
            sliceName = "Slave(" + (projector.ProjectorMesh.Id / 2) + ")." +  
                (projector.stereoType == ProjectorDescription.ProjectorStereoType.Left ? ".LeftProjector" : ".RightProjector")
                + ".SliceCam(" + id + ")";


            cameraObject = new GameObject(sliceName);
            cameraObject.AddComponent<Camera>();
            cameraObject.camera.transform.parent = parent.transform;
            //cameraObject.AddComponent<CameraLines>(); //use this for easy way of getting lines
            // Set up render path
            
			cameraObject.camera.depthTextureMode = DepthTextureMode.Depth;
            cameraObject.camera.renderingPath = RenderingPath.Forward;
            
            
            // We will manually call .render on these cameras
            cameraObject.camera.enabled = false;
            
            this.CopyFrom(copyFrom);
        }

        public void CopyFrom(Camera camera)
        {
            //mainCamera = camera;
            cameraObject.camera.CopyFrom(camera);
            // Rotate camera to its correct position, looking at the angle halfway between its two seams
            cameraObject.camera.transform.Rotate(Vector3.up, Angle);
        }

        public void ComputeProjection(float cameraAngle, float renderRadius, float renderHeight, float eyeWidth, float eyeHeight, float sliceWidth)
        {
            this.cameraAngle = cameraAngle;
            this.renderRadius = renderRadius;
            this.renderHeight = renderHeight;
            this.eyeWidth = eyeWidth;
            this.eyeHeight = eyeHeight;
            this.sliceWidth = sliceWidth;

            float projStart = projector.ProjectorMesh.MeshStart % 1.0f;
            float projEnd = projector.ProjectorMesh.MeshEnd % 1.0f;
//            float projWidth = (projector.ProjectorMesh.MeshStart > projector.ProjectorMesh.MeshEnd)
//                ? (1.0f + projector.ProjectorMesh.MeshEnd - projector.ProjectorMesh.MeshStart)
//                    : (projector.ProjectorMesh.MeshEnd - projector.ProjectorMesh.MeshStart);
            // Translate the camera left or right of itself by half eye width
            cameraObject.camera.transform.localRotation = Quaternion.identity;
            cameraObject.camera.transform.Rotate(Vector3.up, Angle,Space.Self);

            cameraObject.transform.localPosition = Vector3.zero;

            if (projector.stereoType == ProjectorDescription.ProjectorStereoType.Left)
            {
                cameraObject.transform.Translate(Vector3.left / Vector3.left.magnitude * eyeWidth / 2.0f);
            }
            else
            {
                cameraObject.transform.Translate(Vector3.right / Vector3.right.magnitude * eyeWidth / 2.0f);
            }

            // Apply off axis matrix to camera
            float farWidth = 2.0f * renderRadius * Mathf.Sin(Mathf.Deg2Rad * cameraAngle / 2.0f); // formula for a chord length
            float frustumDepth = Mathf.Sqrt(renderRadius * renderRadius - (farWidth / 2.0f) * (farWidth / 2.0f));
            float smallerSide = (cameraObject.camera.nearClipPlane / frustumDepth) * ((farWidth / 2.0f) - (eyeWidth / 2.0f)); // distances from centre line to edge of frustum at near clip plane depth
            float biggerSide = (cameraObject.camera.nearClipPlane / frustumDepth) * ((farWidth / 2.0f) + (eyeWidth / 2.0f));
            float left = 0.0f;
            float right = 0.0f;
            if (projector.stereoType == ProjectorDescription.ProjectorStereoType.Left)
            {
                left = -smallerSide;
                right = biggerSide;
            }
            else
            {
                left = -biggerSide;
                right = smallerSide;
            }

            float top = (cameraObject.camera.nearClipPlane / renderRadius) * (renderHeight - eyeHeight);
            float bottom = -(cameraObject.camera.nearClipPlane / renderRadius) * eyeHeight;

            // If you're not showing the whole slice (the projector starts or ends mid slice)
            // shrink the outside edge of the viewport so that it ends up the same size
            // as the semi-slice
            float projectorEndShifted = (projEnd < projStart ? 1.0f + projEnd : projEnd);

            if (leftSeam < projStart && rightSeam >= projStart)
            {
                //Debug.Log("Camera " + id + " is cut off on the left side.");
                // slice is cut off on the left side
                float distanceIn = projStart - leftSeam;
                float fractionCutOff = distanceIn / sliceWidth;
                left = left + fractionCutOff * (right - left);
                //Debug.Log("Found a left edge on projector " + projector.nodeNumber + ", shrinking");
            }
            if (leftSeam <= projectorEndShifted && rightSeam > projectorEndShifted)
            {
                //Debug.Log("Camera " + id + " is cut off on the right side.");
                // slice is cut off on the right side
                float distanceIn = rightSeam - projectorEndShifted;
                float fractionCutOff = distanceIn / sliceWidth;
                right = right - fractionCutOff * (right - left);
                //Debug.Log("Found a right edge on projector " + projector.nodeNumber + ", shrinking");
            }

            Matrix4x4 projectionMatrix = PerspectiveOffCentre(left, right, bottom, top, cameraObject.camera.nearClipPlane, cameraObject.camera.farClipPlane);
            cameraObject.camera.projectionMatrix = projectionMatrix;

        }

        public void ComputeViewport(float screenPixelsWidth)
        {
            float projStart = projector.ProjectorMesh.MeshStart % 1.0f;
//            float projEnd = projector.ProjectorMesh.MeshEnd % 1.0f;
            float projWidth = (projector.ProjectorMesh.MeshStart > projector.ProjectorMesh.MeshEnd)
                ? (1.0f + projector.ProjectorMesh.MeshEnd - projector.ProjectorMesh.MeshStart)
                    : (projector.ProjectorMesh.MeshEnd - projector.ProjectorMesh.MeshStart);
            // Make camera render its fraction of the view to the corresponding renderplane
            cameraObject.camera.targetTexture = projector._targetRenderTexture;
            float leftViewport = (leftSeam - projStart) / projWidth; // give left and right edges of slice in the render _targetRenderTexture's viewport coordinates
            float rightViewport = (rightSeam - projStart) / projWidth;
            cameraObject.camera.rect = new Rect(leftViewport, 0, (rightViewport - leftViewport + 1.0f / screenPixelsWidth), 1); // draw slice, adding a pixel buffer to ensure slices touch each other
            //newCam.SetActive(false);
        }


        public void SetRenderLayer(int renderLayer)
        {
            cameraObject.camera.cullingMask = (2 << renderLayer);
        }

        public static Matrix4x4 PerspectiveOffCentre(float left, float right, float bottom, float top, float near, float far)
        {
            float x = (2.0f * near) / (right - left);
            float y = (2.0f * near) / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0f * far * near) / (far - near);
            float e = -1.0f;

            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;
            return m;

        }
    }

}