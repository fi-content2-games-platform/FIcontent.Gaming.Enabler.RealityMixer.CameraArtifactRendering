/*==============================================================================
Copyright (c) 2013 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class creates a mesh for a cylinder, cone, or conical frustum.
/// The resulting mesh contains vertex positions, normals, and texture coordinates.
/// The resulting mesh contains inward and outward faces and can therefore be viewed double-sided.
/// The resulting mesh has one, two, or three submeshes: side geometry, top geometry (optional), bottom geometry (optional)
/// </summary>
public class CylinderMeshFactory
{
    private readonly float mTopRadius;
    private readonly float mBottomRadius;
    private readonly float mSideLength;

    //smaller value of top and bottom radius
    private readonly float mSmallRadius;
    //bigger value of top and bottom radius
    private readonly float mBigRadius;
    //texture has to be flipped if top radius is larger than bottom radius
    private readonly bool mFlip;
    //theta is cone angle
    private readonly float mSinTheta;
    //maximum value of u, when origin is at unwrapped cone apex
    private readonly float mUMax;
    //minimum value of v, when origin is at unwrapped cone apex
    private readonly float mVSmall;
    //End of conical frustum, measured from cone apex
    private readonly float mSidelengthBig;
    //Start of conical frustum, measured from cone apex
    private readonly float mSidelengthSmall;

    private List<Vector3> mPositions;
    private List<Vector3> mNormals;
    private List<Vector2> mUVs;

    /// <summary>
    /// Create a mesh for a cylinder, cone, or conical frustum.
    /// </summary>
    /// <param name="sideLength">Distance between point on top circle and corresponding point on bottom circle. Height for cylinders, slant height for cones. </param>
    /// <param name="topDiameter">Top diameter. For an upward cone it is zero. For a cylinder it is equal to the bottom diameter.</param>
    /// <param name="bottomDiameter">Bottom diameter. For a downward cone it is zero. For a cylinder it is equal to the top diameter. </param>
    /// <param name="numPerimeterVertices">Tesselation of the mesh is defined by setting the number of vertices per circle.</param>
    /// <param name="hasTopGeometry">Define if optional top geometry should be generated.</param>
    /// <param name="hasBottomGeometry">Define if optional bottom geometry should be generated.</param>
    /// <returns></returns>
    public static Mesh CreateCylinderMesh(float sideLength, float topDiameter, float bottomDiameter, int numPerimeterVertices, 
        bool hasTopGeometry, bool hasBottomGeometry, bool insideMaterial = false)
    {
        var factory = new CylinderMeshFactory(sideLength, topDiameter, bottomDiameter);
        return factory.CreateCylinderMesh(numPerimeterVertices, hasTopGeometry, hasBottomGeometry, insideMaterial);
    }

    private CylinderMeshFactory(float sideLength, float topDiameter, float bottomDiameter)
    {
        mSideLength = sideLength;
        mTopRadius = topDiameter*0.5f;
        mBottomRadius = bottomDiameter*0.5f;

        //we first assume that the bottom radius is the larger radius
        //otherwise we have to flip the texture coordinates vertically
        mFlip = mTopRadius >= mBottomRadius;
        if (mFlip)
        {
            mBigRadius = mTopRadius;
            mSmallRadius = mBottomRadius;
        }
        else
        {
            mBigRadius = mBottomRadius;
            mSmallRadius = mTopRadius;
        }

        // theta is the cone angle
        if ((mBigRadius - mSmallRadius) >= sideLength) //invalid cylinder
            mSinTheta = 1.0f; 
        else
            mSinTheta = (mBigRadius - mSmallRadius)/sideLength;

        mSidelengthSmall = IsCylinder() ? 0.0f : sideLength*mSmallRadius/(mBigRadius - mSmallRadius);
        mSidelengthBig = mSidelengthSmall + sideLength;

        //angle at apex when the cone is unwrapped to a plane
        float unwrapAngle = Mathf.PI*mSinTheta;
        var isSmallAngle = unwrapAngle < Math.PI/2.0f;
        mUMax = IsCylinder()
                    ? Mathf.PI*mBigRadius
                    : isSmallAngle ? mSidelengthBig*Mathf.Sin(unwrapAngle) : mSidelengthBig;

        mVSmall = Mathf.Cos(unwrapAngle)*(isSmallAngle ? mSidelengthSmall : mSidelengthBig);
    }

    private Mesh CreateCylinderMesh(int numPerimeterVertices, bool hasTopGeometry, bool hasBottomGeometry, bool insideMaterial)
    {
        mPositions = new List<Vector3>();
        mNormals = new List<Vector3>();
        mUVs = new List<Vector2>();

        var height = ComputeHeight(mSideLength);

        var bottomPerimeterVertices = CreatePerimeterPositions(0f, mBottomRadius, numPerimeterVertices);
        var topPerimeterVertices = CreatePerimeterPositions(height, mTopRadius, numPerimeterVertices);

        var bodyTriangles = AddBodyTriangles(bottomPerimeterVertices, topPerimeterVertices);
        var numSubMeshes = 1;

        //add bottom geometry
        List<int> bottomTriangles = null;
        if (hasBottomGeometry && mBottomRadius > 0f)
        {
            bottomTriangles = AddSealingTriangles(bottomPerimeterVertices, false);
            numSubMeshes++;
        }

        //add top geometry
        List<int> topTriangles = null;
        if (hasTopGeometry && mTopRadius > 0f)
        {
            topTriangles = AddSealingTriangles(topPerimeterVertices, true);
            numSubMeshes++;
        }

        //assemble mesh
        var mesh = new Mesh
        {
            vertices = mPositions.ToArray(),
            normals = mNormals.ToArray(),
            uv = mUVs.ToArray(),
            subMeshCount = insideMaterial ? 2 * numSubMeshes : numSubMeshes
        };


        int[] insideBodyTriangles = null;
        int[] insideTopTriangles = null;
        int[] insideBottomTriangles = null;
        if (insideMaterial)
        {
            //divide all triangle lists into inside (white, last submesh) and outside (other submeshes)
            insideBodyTriangles = bodyTriangles.Skip(bodyTriangles.Count / 2).ToArray();
            bodyTriangles = bodyTriangles.Take(bodyTriangles.Count / 2).ToList();
            if (bottomTriangles != null)
            {
                insideBottomTriangles = bottomTriangles.Take(bottomTriangles.Count / 2).ToArray();
                bottomTriangles = bottomTriangles.Skip(bottomTriangles.Count / 2).ToList();
            }
            if (topTriangles != null)
            {
                insideTopTriangles = topTriangles.Skip(topTriangles.Count / 2).ToArray();
                topTriangles = topTriangles.Take(topTriangles.Count / 2).ToList();
            }
        }


        //create separate triangle sets for bottom and top for enabling different materials
        mesh.SetTriangles(bodyTriangles.ToArray(), 0);
        if (bottomTriangles != null)
            mesh.SetTriangles(bottomTriangles.ToArray(), 1);
        if (topTriangles != null)
            mesh.SetTriangles(topTriangles.ToArray(), numSubMeshes - 1);


        if (insideMaterial)
        {
            mesh.SetTriangles(insideBodyTriangles.ToArray(), numSubMeshes);
            if (insideBottomTriangles != null)
                mesh.SetTriangles(insideBottomTriangles, numSubMeshes + 1);
            if (insideTopTriangles != null)
                mesh.SetTriangles(insideTopTriangles, numSubMeshes + numSubMeshes - 1);
        }


        return mesh;
    }

    private List<int> AddBodyTriangles(List<Vector3> bottomPerimeterVertices, List<Vector3> topPerimeterVertices)
    {
        int numPerimeterVertices = bottomPerimeterVertices.Count;
        var sideNormals = new List<Vector3>();

        //compute all side normals
        for (int i = 0; i < numPerimeterVertices; i++)
        {
            var bottomVtx = bottomPerimeterVertices[i];
            var topVtx = topPerimeterVertices[i];
            var radialDirection = new Vector3(bottomVtx.x + topVtx.x, 0f, bottomVtx.z + topVtx.z);
            radialDirection.Normalize();
            var tangent = Vector3.Cross(Vector3.up, radialDirection) * -1;

            var bottomToTop = topVtx - bottomVtx;
            var angle = Vector3.Angle(Vector3.up, bottomToTop);

            var rotation = Quaternion.AngleAxis(angle, tangent);
            var normal = rotation * radialDirection;
            sideNormals.Add(normal);
        }

        //assemble bottom vertices and normals and create texture coordinates        
        var currentAngle = -Mathf.PI;
        var stepAngle = (2 * Mathf.PI) / numPerimeterVertices;
        for (int i = 0; i <= numPerimeterVertices; i++)
        {
            //first vertex is added twice for creating seam between left and right boundary of texture
            var idx = i % numPerimeterVertices;
            mPositions.Add(bottomPerimeterVertices[idx]);
            mNormals.Add(sideNormals[idx]);
            mUVs.Add(ConvertToUVCoordinates(currentAngle, 0.0f));
            currentAngle += stepAngle;
        }

        //assemble top vertices and normals and create texture coordinates   
        currentAngle = -Mathf.PI;
        for (var i = 0; i <= numPerimeterVertices; i++)
        {
            var idx = i % numPerimeterVertices;
            mPositions.Add(topPerimeterVertices[idx]);
            mNormals.Add(sideNormals[idx]);
            mUVs.Add(ConvertToUVCoordinates(currentAngle, mSideLength));
            currentAngle += stepAngle;
        }

        //duplicate all vertices for setting inward-normals
        var inwardOffset = mPositions.Count;
        for (var i = 0; i < mPositions.Count; i++)
            mNormals.Add(-mNormals[i]);
        mPositions.AddRange(mPositions);        
        mUVs.AddRange(mUVs);
        
               

        //create triangles for body
        var bodyTriangles = new List<int>();
        for (var i = 1; i <= numPerimeterVertices; i++)
        {
            // outwards facing
            bodyTriangles.AddRange(new[] {i - 1, i, i + numPerimeterVertices});
            bodyTriangles.AddRange(new[] {i + numPerimeterVertices + 1, i + numPerimeterVertices, i});
        }
        for (var i = 1; i <= numPerimeterVertices; i++)
        {
            // inwards facing
            bodyTriangles.AddRange(new[] { inwardOffset + i - 1, inwardOffset + i + numPerimeterVertices, inwardOffset + i });
            bodyTriangles.AddRange(new[] { inwardOffset + i + numPerimeterVertices + 1, inwardOffset + i, inwardOffset + i + numPerimeterVertices });
        }

        return bodyTriangles;
    }

    /// <summary>
    /// Create a circle for top or bottom geometry. Positions, normals, and texture coordinates are added.
    /// </summary>
    /// <param name="perimeterVertices">Vertices of circle, must be parallel to xz-plane</param>
    /// <param name="isTop">define whether the circle is for the top or for the bottom geometry</param>
    /// <returns>return face indices</returns>
    private List<int> AddSealingTriangles(List<Vector3> perimeterVertices, bool isTop)
    {
        var triangles = new List<int>();

        for (var inOut = 0; inOut < 2; inOut++)
        {
            var upward = inOut == 0;
            var normal = upward ? Vector3.up : Vector3.down;

            var centerIdx = mPositions.Count;
            mPositions.Add(new Vector3(0, perimeterVertices[0].y, 0));

            var startingIdx = mPositions.Count;
            var finishingIdx = startingIdx + perimeterVertices.Count - 1;

            mPositions.AddRange(perimeterVertices);

            // add one normal for each perimeter vertex + the center one
            for (var i = 0; i <= perimeterVertices.Count; i++)
            {
                mNormals.Add(normal);
            }

            mUVs.AddRange(CreatePerimeterUVCoordinates(perimeterVertices.Count, isTop));

            // create triangles
            for (var i = startingIdx; i < finishingIdx; i++)
            {
                triangles.AddRange(upward
                                       ? new[] {centerIdx, i, i + 1}
                                       : new[] {centerIdx, i + 1, i});
            }

            // final triangle
            triangles.AddRange(upward
                                   ? new[] {centerIdx, finishingIdx, startingIdx}
                                   : new[] {centerIdx, startingIdx, finishingIdx});
        }
        return triangles;
    }

    /// <summary>
    /// Create positions for a circle at a specific height (y-coordinate).
    /// Note that the real height (y-coordinate) is used, not the sidelength
    /// </summary>
    private static List<Vector3> CreatePerimeterPositions(float height, float radius, int numPerimeterVertices)
    {
        var vertices = new List<Vector3>();

        var angle = -Mathf.PI * 0.5f;
        var stepAngle = 2 * Mathf.PI / numPerimeterVertices;
        for (var i = 0; i < numPerimeterVertices; i++)
        {
            vertices.Add(new Vector3(radius * Mathf.Sin(angle), height, radius * Mathf.Cos(angle)));
            angle += stepAngle;
        }

        return vertices;
    }

    /// <summary>
    /// Create texture coordinates for top or bottom geometry
    /// </summary>
    private static List<Vector2> CreatePerimeterUVCoordinates(int numPerimeterVertices, bool isTop)
    {
        var uvCoords = new List<Vector2>();

        //first vertex is center
        uvCoords.Add(new Vector2(0.5f, 0.5f));

        float angle = -Mathf.PI * 0.5f;
        float stepAngle = (Mathf.PI * 2) / numPerimeterVertices;
        for (int i = 0; i < numPerimeterVertices; i++)
        {
            var x = Mathf.Cos(angle) * 0.5f + 0.5f;
            var y = Mathf.Sin(angle) * 0.5f + 0.5f;
            x = 1 - x;
            if(!isTop)
                y = 1 - y;
            uvCoords.Add(new Vector2(x, y));
            angle += stepAngle;
        }
        return uvCoords;
    }


    /// <summary>
    /// Convert a 3D-position to a texture coordinate for the side geometry.
    /// </summary>
    /// <param name="angleInRadians">angle in xz-plane</param>
    /// <param name="slantedYPos">Slant height (sidelength) of 3D position</param>
    /// <returns>Texture coordinate within range [0,1]</returns>
    private Vector2 ConvertToUVCoordinates(float angleInRadians, float slantedYPos)
    {
        if (mFlip)
        {
            angleInRadians = -angleInRadians;
            slantedYPos = mSidelengthSmall + slantedYPos;
        }
        else
        {
            slantedYPos = mSidelengthBig - slantedYPos;
        }


        float oX, oY;
        if (IsCylinder())
        {
            oX = angleInRadians * mBigRadius;
            oY = slantedYPos;
        }
        else //cone/conical frustum
        {
            oX = slantedYPos * Mathf.Sin(angleInRadians * mSinTheta);
            oY = slantedYPos * Mathf.Cos(angleInRadians * mSinTheta);
        }

        oX = oX / (2.0f * mUMax) + 0.5f;
        oY = (oY - mVSmall) / (mSidelengthBig - mVSmall);

        if (mFlip)
        {
            oY = 1 - oY;
            oX = 1 - oX;
        }
        return new Vector2(oX, oY);
    }

    private bool IsCylinder()
    {
        return Math.Abs(mBigRadius - mSmallRadius) < 1e-5;
    }

    private float ComputeHeight(float sideLength)
    {
        return Mathf.Sqrt(sideLength*sideLength - (mBigRadius - mSmallRadius)*(mBigRadius - mSmallRadius));
    }
}
