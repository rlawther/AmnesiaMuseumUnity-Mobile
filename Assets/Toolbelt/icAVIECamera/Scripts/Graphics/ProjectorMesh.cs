using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.AVIE.Scripts.Graphics
{
    public class ProjectorMesh
    {
        private int id;
        private int gridWidth;
        private int gridHeight;
        private int numVertices;
        private float meshStart;
        private float meshEnd;

        public int Id
        {
            get 
            {
                return id; 
            }
        }

        public int GridWidth
        {
            get
            {
                return gridWidth;
            }
        }

        public int GridHeight
        {
            get
            {
                return gridHeight;
            }
        }

        public int NumVertices
        {
            get
            {
                return numVertices;
            }
        }

        public float MeshStart
        {
            get
            {
                return meshStart;
            }
        }

        public float MeshEnd
        {
            get
            {
                return meshEnd;
            }
        }

        public class CylinderPoint
        {
            // x is the position between meshStart and meshEnd and y is the height
            public Vector2 xy;
            public Vector2 uv;

            public CylinderPoint(Vector2 xy, Vector2 uv)
            {
                this.xy = xy;
                this.uv = uv;
            }
        }

        private CylinderPoint[] points;

        public ProjectorMesh(int id, int gridWidth, int gridHeight, float meshStart, float meshEnd)
        {
            this.id = id;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.numVertices = (gridWidth + 1) * (gridHeight + 1);
            this.meshStart = meshStart;
            this.meshEnd = meshEnd;
            points = new CylinderPoint[numVertices];
        }

        public void SetCylinderPoint(int xIndex, int yIndex, Vector2 xy, Vector2 uv)
        {
            points[yIndex * (gridWidth + 1) + xIndex] = new CylinderPoint(xy, uv);
        }

        public CylinderPoint GetCylinderPoint(int index)
        {
            return points[index];
        }
    }
}
