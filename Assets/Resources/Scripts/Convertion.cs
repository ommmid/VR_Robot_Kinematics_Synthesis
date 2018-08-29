using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convertion : MonoBehaviour {
    // these functions work in right hand system with y upward. So when I want to use them I need to change the coordinates of 
    // whatever entity I am gonna use to right hand z up coordinates and then use these functions
    public float vecNorm(Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
    } // ------------ correct

    public Vector3 vecUnit(Vector3 vec)
    {
        float vN = vecNorm(vec);
        Vector3 vecU = new Vector3(vec.x / vN, vec.y / vN, vec.z / vN);
        return vecU;
    } // ------------ correct

    public float DotProduct(Vector3 a, Vector3 b)
    {
        float dt = a.x * b.x + a.y * b.y + a.z * b.z;
        return dt;
    } // ------------ correct

    public Vector3 CrossProduct(Vector3 v1, Vector3 v2)
    {
        float x, y, z;
        x = v1.y * v2.z - v2.y * v1.z;
        y = (v1.x * v2.z - v2.x * v1.z) * -1;
        z = v1.x * v2.y - v2.x * v1.y;

        return new Vector3(x, y, z);
    } // ------------ correct

    public float[,] mat3Mult(float[,] ar1, float[,] ar2)
    {
        float[,] arMult = new float[3, 3];
        arMult[0, 0] = ar1[0, 0] * ar2[0, 0] + ar1[0, 1] * ar2[1, 0] + ar1[0, 2] * ar2[2, 0];
        arMult[0, 1] = ar1[0, 0] * ar2[0, 1] + ar1[0, 1] * ar2[1, 1] + ar1[0, 2] * ar2[2, 1];
        arMult[0, 2] = ar1[0, 0] * ar2[0, 2] + ar1[0, 1] * ar2[1, 2] + ar1[0, 2] * ar2[2, 2];

        arMult[1, 0] = ar1[1, 0] * ar2[0, 0] + ar1[1, 1] * ar2[1, 0] + ar1[1, 2] * ar2[2, 0];
        arMult[1, 1] = ar1[1, 0] * ar2[0, 1] + ar1[1, 1] * ar2[1, 1] + ar1[1, 2] * ar2[2, 1];
        arMult[1, 2] = ar1[1, 0] * ar2[0, 2] + ar1[1, 1] * ar2[1, 2] + ar1[1, 2] * ar2[2, 2];

        arMult[2, 0] = ar1[2, 0] * ar2[0, 0] + ar1[2, 1] * ar2[1, 0] + ar1[2, 2] * ar2[2, 0];
        arMult[2, 1] = ar1[2, 0] * ar2[0, 1] + ar1[2, 1] * ar2[1, 1] + ar1[2, 2] * ar2[2, 1];
        arMult[2, 2] = ar1[2, 0] * ar2[0, 2] + ar1[2, 1] * ar2[1, 2] + ar1[2, 2] * ar2[2, 2];

        return arMult;
    } // -------------- correct

    // convert homegenous matrix from Unity to MATLAB
    public float[,] conU2M_H(Matrix4x4 m)
    {
        // get the rotation part
        float[,] rot3 = { { m[0, 0], m[0, 1], m[0, 2] }, { m[1, 0], m[1, 1], m[1, 2] }, { m[2, 0], m[2, 1], m[2, 2] } };
        // get the translation part as matrix
        float[,] b = { { 0, 0, m[0, 3] }, { 0, 0, m[1, 3] }, {0, 0, m[2, 3] } };
        // reflection matrix
        float[,] Sz = { { 1, 0, 0 }, { 0, 0, 1 }, { 0, 1, 0 } };
        // convert a rotation matrix from Unity to MATLAB is the same as MATLAB to Unity
        // multiply the rotation matrix by the reflection matrix before and after 
        float[,] Szrot3 = mat3Mult(Sz, rot3);
        float[,] Szrot3Sz = mat3Mult(Szrot3, Sz);
        // convert the translation part
        float[,] Szb = mat3Mult(Sz, b);
        // the final converted matrix is:
        float[,] mat = new float[4, 4];
        mat[0, 0] = Szrot3Sz[0, 0]; mat[0, 1] = Szrot3Sz[0, 1]; mat[0, 2] = Szrot3Sz[0, 2]; mat[0, 3] = Szb[0, 2];
        mat[1, 0] = Szrot3Sz[1, 0]; mat[1, 1] = Szrot3Sz[1, 1]; mat[1, 2] = Szrot3Sz[1, 2]; mat[1, 3] = Szb[1, 2];
        mat[2, 0] = Szrot3Sz[2, 0]; mat[2, 1] = Szrot3Sz[2, 1]; mat[2, 2] = Szrot3Sz[2, 2]; mat[2, 3] = Szb[2, 2];
        mat[3, 0] = 0; mat[3, 1] = 0; mat[3, 2] = 0; mat[3, 3] = 1;

        return mat;
    } // -------------- correct

    public Quaternion qMult(Quaternion q1, Quaternion q2)
    {
        // in Unity: Quaternion=[vx, vy, vz, s1]
        Vector3 vec1 = new Vector3(q1.x, q1.y, q1.z);
        Vector3 vec2 = new Vector3(q2.x, q2.y, q2.z);

        Quaternion quat = new Quaternion();
        quat.x = q1.w * vec2.x + q2.w * vec1.x + CrossProduct(vec1, vec2).x;
        quat.y = q1.w * vec2.y + q2.w * vec1.y + CrossProduct(vec1, vec2).y;
        quat.z = q1.w * vec2.z + q2.w * vec1.z + CrossProduct(vec1, vec2).z;
        quat.w = q1.w * q2.w - DotProduct(vec1, vec2);
        return quat;
    } // ---------------- correct

    // convert Unity quaternion to Matlab quaternion
    public Quaternion conU2M_quaternion(Quaternion quaternionUnity)
    {
        // if we have a quaternion like this in UNity: Quaternion.AngleAxis(30, new Vector3(0.2673f, 0.5345f, 0.8018f));
        // then its counterpart in MATLAB is:          UnitQuaternion.angvec(-30*pi/180,   [0.2673,  0.8018,  0.5345])
        // which means: switch y with z and make the angle of rotation negative
        // or keep the scalar part the same and make the vecotr part negative and also swith its z element with y element

        // Quaternion(float x, float y, float z, float w)
        Quaternion quaternionMATLAB = new Quaternion(-quaternionUnity.x, -quaternionUnity.z, -quaternionUnity.y, quaternionUnity.w);

        return quaternionMATLAB;
    } // ---------------- correct

    // convert a point from Unity to MATLAB and vice versa
    public Vector3 conU2M_translation(Vector3 h)
    {
        // this would work for points, directions and any vector
        // just switch z and y
        return new Vector3(h[0], h[2], h[1]);
    } // ---------------- correct

    // (In MATLAB coordinates) having the rotation in quaternion form and the location as a vetor, we want to calulate the dual quaternion 
    public float[] qv2DQ(Quaternion q, Vector3 v)
    {
        // ArtTreeKS form => DQ = (a + V) + e(V0 + a0)
        float[] dq = new float[8];
        dq[0] = q.w; dq[1] = q.x; dq[2] = q.y; dq[3] = q.z;

        // Quaternion(float x, float y, float z, float w)
        Quaternion p = new Quaternion(v.x, v.y, v.z, 0);
        Quaternion qp = qMult(p, q);
        dq[4] = qp.x /2; dq[5] = qp.y/2; dq[6] = qp.z/2; dq[7] = qp.w/2;

        return dq;
    } // ------------------- correct


    // convert a Homogeneous matrix to a dual quaternion
    public float[] conHM2DQ(float[,] m)
    {
        // in Unity Quaternion=[vx, vy, vz, s1]
        // consider the homogeneous matrix constructed by a translation d and then rotation Q
        Vector3 n = new Vector3();

        float rd = (float)(m[0, 0] + m[1, 1] + m[2, 2] - 1) / (float)(2);
        float t = Mathf.Acos(rd); // angle between 0 and pi
        if (t >= 0.003f)
        {
            n = new Vector3((float)((float)(m[2, 1] - m[1, 2]) / (float)Mathf.Sin(t)) / (float)2,
                            (float)((float)(m[0, 2] - m[2, 0]) / (float)Mathf.Sin(t)) / (float)2,
                            (float)((float)(m[1, 0] - m[0, 1]) / (float)Mathf.Sin(t)) / (float)2);
        }

        // ArtTreeKS form => DQ = (a + V) + e(V0 + a0)
        float[] dq = new float[8];
        Quaternion q = new Quaternion(Mathf.Sin((float)t / (float)2) * n.x,
                                      Mathf.Sin((float)t / (float)2) * n.y,
                                      Mathf.Sin((float)t / (float)2) * n.z,
                                      Mathf.Cos((float)t / (float)2));
        dq[0] = q.w; dq[1] = q.x; dq[2] = q.y; dq[3] = q.z;
        // get the translation from homogenous matrix. 
        Vector3 v = new Vector3(m[0, 3], m[1, 3], m[2, 3]); // do I need to add (f) to convert to float for m[,]
        //This is not the same as translation along screw axis.I have to calculate that
        float vn = DotProduct(v, n);
        Quaternion ps = new Quaternion( vn * n.x, vn * n.y, vn * n.z, 0); // translation as a vector along the screw axis
        Quaternion qp = qMult(ps, q);
        dq[4] = (float)qp.x/ (float)2; dq[5] = (float)qp.y/ (float)2; dq[6] = (float)qp.y/ (float)2; dq[7] = (float)qp.w/ (float)2;

        return dq;
    } // ----------- is not correct but I have to work on it


    //--------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------

 

    

// -------- I am not sure about the rest


float[] makeDQ(Quaternion q, Vector3 r)
{
    // having the axis of rotation plus the translation, I want to make the DQ in 
    // the form of ArtTreeKS
    // DQ = (a+V) + e (v0+a0)

    Vector3 vec = new Vector3(q[0], q[1], q[2]);
    Vector3 n = vecUnit(vec);
    float rn = DotProduct(r, n);
    Quaternion pb = new Quaternion(rn * n.x, rn * n.y, rn * n.z, 0);
    Quaternion qpb = qMult(q, pb);
    // Quaternion =[vx, vy, vz, s1]

    float[] DQ = { q[3], q[0], q[1], q[2], qpb[0], qpb[1], qpb[2], qpb[3] };
    return DQ;
}




    float[,] conH2DQ(float[,] h)
    {
        // convert 4x4 homogenous matrix h to Dual Quaternion

        float[,] DQ = new float[1, 8];
        return DQ;
    }
}
