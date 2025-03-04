using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    internal static class PolylineLength
    {
        // Crazy SIMD math decompiled with IDA.
        // I don't know which algorithm this is so I'm unable to refactor this
        // into a more readable form, but the output is correct and that's
        // good enough for me.

        /// <summary>
        /// Calculates the cached length property of one polyline item.
        /// </summary>
        /// <param name="bwNode">The backward node of the item.</param>
        /// <param name="fwNode">The forward node of the item.</param>
        /// <returns>The length.</returns>
        public static float Calculate(INode bwNode, INode fwNode)
        {
            float length; // xmm13_4
            float deltaX; // er10
            Vector128<float> v5; // xmm4
            float deltaY; // ebx
            long i; // rsi
            float deltaZ; // edi
            Vector128<float> v9; // xmm8
            Vector128<float> v10; // xmm9
            Vector128<float> bwRot; // xmm11
            Vector128<float> fwRot; // xmm12
            Vector128<float> v13; // xmm0
            Vector128<float> v14; // xmm1
            Vector128<float> invFwRot; // xmm15
            Vector128<float> v16; // xmm7
            Vector128<float> invBwRot; // xmm14
            Vector128<float> v18; // xmm11
            float bwRotW; // xmm2_4
            Vector128<float> v20; // xmm12
            Vector128<float> v21; // xmm3
            Vector128<float> v22; // xmm4
            Vector128<float> v23; // xmm2
            Vector128<float> v24; // xmm10
            Vector128<float> v25; // xmm4
            Vector128<float> v26; // xmm3
            Vector128<float> v27; // xmm10
            Vector128<float> v28; // xmm6
            Vector128<float> v29; // xmm7
            Vector128<float> v30; // xmm3
            Vector128<float> v31; // xmm3
            Vector128<float> v32; // xmm3
            Vector128<float> v33; // xmm2
            Vector128<float> v34; // xmm1
            Vector128<float> v35; // xmm2
            Vector128<float> v36; // xmm2
            Vector128<float> v37; // xmm3
            Vector128<float> v38; // xmm2
            Vector128<float> v39; // xmm0
            Vector128<float> v40; // xmm1
            Vector128<float> v41; // xmm1
            Vector128<float> v42; // xmm1
            Vector128<float> v43; // xmm1

            Vector128<float> v45; // [rsp+48h] [rbp-C0h] BYREF
            float bwRotW_2; // [rsp+58h] [rbp-B0h]
            Vector128<float> v47 = new(); // [rsp+68h] [rbp-A0h]
            Vector128<float> v48 = new(); // [rsp+78h] [rbp-90h]
            Vector128<float> v49 = new(); // [rsp+88h] [rbp-80h]
            Vector128<float> v50; // [rsp+98h] [rbp-70h]
            Vector128<float> v51; // [rsp+A8h] [rbp-60h] BYREF
            Vector128<float> v52; // [rsp+B8h] [rbp-50h] BYREF
            Vector128<float> v53; // [rsp+C8h] [rbp-40h] BYREF
            Vector128<float> v54; // [rsp+D8h] [rbp-30h]
            Vector128<float> v55; // [rsp+E8h] [rbp-20h]
            Vector128<float> v56; // [rsp+F8h] [rbp-10h]

            var v5Initial = Vector128.Create(0f, 0f, 0f, -1f);
            var v9Initial = Vector128.Create(-1f, -1f, 0, 0);
            var v10Initial = Vector128.Create(0f, 0f, -1f, 0f);

            length = 3.0f;
            deltaX = fwNode.Position.X - bwNode.Position.X;
            deltaY = fwNode.Position.Y - bwNode.Position.Y;
            deltaZ = fwNode.Position.Z - bwNode.Position.Z;
            v5 = v5Initial;
            v9 = v9Initial;
            v10 = v10Initial;
            bwRot = Vector128.Create(bwNode.Rotation.W, bwNode.Rotation.X, bwNode.Rotation.Y, bwNode.Rotation.Z);
            fwRot = Vector128.Create(fwNode.Rotation.W, fwNode.Rotation.X, fwNode.Rotation.Y, fwNode.Rotation.Z);
            v13 = Sse.Shuffle(bwRot, bwRot, 1);
            v14 = Sse.Shuffle(bwRot, bwRot, 251);
            invFwRot = Sse.Subtract(Vector128<float>.Zero, fwRot);
            v16 = Sse.Shuffle(fwRot, fwRot, 1);
            v54 = v13;
            invBwRot = Sse.Subtract(Vector128<float>.Zero, bwRot);
            v55 = v14;
            v18 = Sse.Shuffle(bwRot, bwRot, 156);
            v56 = v16;
            bwRotW = -invBwRot[0];
            v50 = Sse.Shuffle(fwRot, fwRot, 251);
            bwRotW_2 = -invBwRot[0];
            v20 = Sse.Shuffle(fwRot, fwRot, 156);

            for (i = 4; i > 0; i--)
            {
                v21 = invBwRot;
                v21 = v21.WithElement(0, bwRotW);
                v22 = Sse.Subtract(
                    Sse.Add(Sse.Multiply(v5, v13), Sse.Multiply(v9, v14)),
                    Sse.Multiply(v10, v18));
                v22 = v22.WithElement(0, -v22[0]);
                v23 = Sse.Multiply(Sse.Shuffle(v22, v22, 251), Sse.Shuffle(v21, v21, 31));
                v24 = Sse.Add(
                    Sse.Multiply(Sse.Shuffle(v22, v22, 1), Sse.Shuffle(v21, v21, 229)),
                    Sse.Multiply(Sse.Shuffle(v22, v22, 102), Sse.Shuffle(v21, v21, 130)));
                v25 = Sse.Multiply(Sse.Shuffle(v22, v22, 156), Sse.Shuffle(v21, v21, 120));
                v26 = invFwRot;
                v27 = Sse.Subtract(Sse.Add(v24, v23), v25);
                v27 = v27.WithElement(0, -v27[0]);
                v28 = Sse.Subtract(
                    Sse.Add(Sse.Multiply(v5Initial, v16),
                    Sse.Multiply(v9Initial, v50)),
                    Sse.Multiply(v10Initial, v20));
                v28 = v28.WithElement(0, -v28[0]);
                v26 = v26.WithElement(0, -invFwRot[0]);
                v29 = Sse.Subtract(
                    Sse.Add(
                        Sse.Multiply(Sse.Shuffle(v28, v28, 251), Sse.Shuffle(v26, v26, 31)),
                        Sse.Add(
                            Sse.Multiply(Sse.Shuffle(v28, v28, 1), Sse.Shuffle(v26, v26, 229)),
                            Sse.Multiply(Sse.Shuffle(v28, v28, 102), Sse.Shuffle(v26, v26, 130)))),
                    Sse.Multiply(Sse.Shuffle(v28, v28, 156), Sse.Shuffle(v26, v26, 120)));
                v29 = v29.WithElement(0, -v29[0]);
                v30 = Sse.Shuffle(v47, v47, 210);
                v30 = v30.WithElement(0, deltaZ);
                v53 = Vector128<float>.Zero;
                v31 = Sse.Shuffle(v30, v30, 198);
                v31 = v31.WithElement(0, deltaY);
                v32 = Sse.Shuffle(v31, v31, 225);
                v32 = v32.WithElement(0, deltaX);
                v33 = Sse.Shuffle(v49, v49, 210);
                v34 = Sse.Multiply(v32, v32);
                v33 = v33.WithElement(0, (float)(Sse.Shuffle(v29, v29, 255)[0] * length / 3));
                v35 = Sse.Shuffle(v33, v33, 198);
                v35 = v35.WithElement(0, (float)(Sse.Shuffle(v29, v29, 170)[0] * length / 3));
                v47 = v32;
                v36 = Sse.Shuffle(v35, v35, 225);
                v45 = v32;
                v36 = v36.WithElement(0, (float)(Sse.Shuffle(v29, v29, 85)[0] * length / 3));
                v49 = v36;
                v37 = Sse.Subtract(v32, v36);
                v38 = Sse.Multiply(v36, v36);
                v39 = Sse.Add(Sse.Add(Sse.Shuffle(v34, v34, 73), v34), Sse.Shuffle(v34, v34, 146));
                v40 = Sse.Shuffle(v48, v48, 210);
                v40 = v40.WithElement(0, (float)(Sse.Shuffle(v27, v27, 255)[0] * length / 3));
                v51 = v37;
                v41 = Sse.Shuffle(v40, v40, 198);
                v41 = v41.WithElement(0, (float)(Sse.Shuffle(v27, v27, 170)[0] * length / 3));
                v42 = Sse.Shuffle(v41, v41, 225);
                v42 = v42.WithElement(0, (float)(Sse.Shuffle(v27, v27, 85)[0] * length / 3));
                v48 = v42;
                v52 = v42;
                v43 = Sse.Multiply(v42, v42);

                float a5 = Sse.Sqrt(Sse.Add(Sse.Add(Sse.Shuffle(v43, v43, 73), v43), Sse.Shuffle(v43, v43, 146)))[0];
                float a6 = Sse.Sqrt(Sse.Add(Sse.Add(Sse.Shuffle(v38, v38, 73), v38), Sse.Shuffle(v38, v38, 146)))[0];
                float a7 = (float)(Sse.Sqrt(v39)[0] / 300.0);
                length = Sub14106F6C0(ref v53, ref v52, ref v51, ref v45, a5, a6, a7);

                v13 = v54;
                bwRotW = bwRotW_2;
                v14 = v55;
                v16 = v56;
                v5 = v5Initial;
                v9 = v9Initial;
                v10 = v10Initial;
            }

            return (int)(float)(length * 10000.0) / 10000.0f;
        }

        private static float Sub14106F6C0(ref Vector128<float> a1, ref Vector128<float> a2,
            ref Vector128<float> a3, ref Vector128<float> a4, float a5, float a6, float a7)
        {
            //Vector128<float> v8; // xmm7
            Vector128<float> v9; // xmm1
            Vector128<float> v10; // xmm1
            Vector128<float> v11; // xmm0
            Vector128<float> v12; // xmm1
            float v13; // xmm3_4
            Vector128<float> v14; // xmm1
            float v15; // xmm4_4
            Vector128<float> v17; // xmm0
            Vector128<float> v18; // xmm1
            Vector128<float> v19; // xmm5
            Vector128<float> v20; // xmm5
            float v21; // xmm8_4
            float v22; // xmm6_4
            Vector128<float> v23; // [rsp+40h] [rbp-B8h] BYREF
            Vector128<float> v24; // [rsp+50h] [rbp-A8h] BYREF
            Vector128<float> v25; // [rsp+60h] [rbp-98h] BYREF
            Vector128<float> v26; // [rsp+70h] [rbp-88h] BYREF
            Vector128<float> v27; // [rsp+80h] [rbp-78h] BYREF

            var onehalfs = Vector128.Create(0.5f);

            v9 = Sse.Subtract(a4, a1);
            v10 = Sse.Multiply(v9, v9);
            v11 = Sse.Add(Sse.Add(Sse.Shuffle(v10, v10, 73), v10), Sse.Shuffle(v10, v10, 146));
            v12 = Sse.Subtract(a3, a2);
            v13 = Sse.Sqrt(v11)[0];
            v14 = Sse.Multiply(v12, v12);
            v15 = (float)(Sse.Sqrt(Sse.Add(
                Sse.Add(Sse.Shuffle(v14, v14, 73), v14),
                Sse.Shuffle(v14, v14, 146)))[0] + a5) + a6;

            if ((v15 - v13) < Math.Max(a7, 0.0000099999997))
                return (v13 + v15) * 0.5f;

            v17 = Sse.Add(a2, a1);
            v18 = Sse.Multiply(Sse.Add(a3, a2), onehalfs);
            v27 = Sse.Multiply(Sse.Add(a3, a4), onehalfs);
            v25 = Sse.Multiply(v17, onehalfs);
            v26 = Sse.Multiply(Sse.Add(v27, v18), onehalfs);
            v24 = Sse.Multiply(Sse.Add(v18, v25), onehalfs);
            v19 = Sse.Subtract(v26, v24);
            v20 = Sse.Multiply(v19, v19);
            v23 = Sse.Multiply(Sse.Add(v26, v24), onehalfs);
            v21 = Sse.Sqrt(Sse.Add(Sse.Add(Sse.Shuffle(v20, v20, 73), v20), Sse.Shuffle(v20, v20, 146)))[0] * 0.5f;

            v22 = Sub14106F6C0(ref a1, ref v25, ref v24, ref v23, a5 * 0.5f, v21, a7 * 0.5f);
            return Sub14106F6C0(ref a4, ref v27, ref v26, ref v23, a6 * 0.5f, v21, a7 * 0.5f) + v22;
        }
    }
}
