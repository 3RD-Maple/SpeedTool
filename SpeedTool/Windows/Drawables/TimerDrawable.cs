using System.Numerics;
using SpeedTool.Global;
using SpeedTool.Global.Definitions;

namespace SpeedTool.Windows.Drawables;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SpeedTool.Timer;
using SpeedTool.Util.GL;
using SpeedTool.Util;

using STShader = Util.GL.Shader;

sealed class TimerDrawable : IDisposable
{
    
    public Vector4 SecondsColor { get; set; }
    public Vector4 MinutesColor { get; set; }
    public Vector4 HoursColor { get; set; }
    public TimerDrawable(GL gl)
    {
        s = new STShader(vertCode, fragCode, gl);
        objects = new DrawObject[3];
        objects[0] = CreateDrawObject(gl, 1.0f, 0.1f);
        objects[1] = CreateDrawObject(gl, 0.9f, 0.1f);
        objects[2] = CreateDrawObject(gl, 0.8f, 0.1f);

        var no = CreateVerticesNotches(1.0f, 0.3f);
        notches = new DrawObject(gl, no.Item1, no.Item2);
        this.gl = gl;
    }

    static private DrawObject CreateDrawObject(GL gl, float rad, float thick)
    {
        var res = CreateVertices(rad, thick);
        return new DrawObject(gl, res.Item1, res.Item2);
    }

    static (float[], ushort[]) CreateVertices(float radius, float thickness)
    {
        const double M_PI = 3.14159265358979323846;
        return Arch(radius, thickness, (float)M_PI / 2.0f, -3.0f * (float)M_PI / 2.0f);
    }

    static (float[], ushort[]) CreateVerticesNotches(float radius, float thickness)
    {
        const double STEP = Math.PI / 6;
        List<float> verices = new();
        List<ushort> indices = new();
        for(int i = 0; (i * STEP) < (2 * Math.PI); i++)
        {
            var dsta = (float)(i * STEP);
            var dstb = (float)(dsta - 0.003f);
            var n = Arch(radius, thickness, dsta, dstb);
            verices.AddRange(n.Item1);
            indices.AddRange(n.Item2);
        }

        return (verices.ToArray(), indices.ToArray());
    }

    static (float[], ushort[]) Arch(float radius, float thickness, float beg, float end, float z = 0.0f)
    {
        List<float> ret = new List<float>();
        var cos = Math.Cos;
        var sin = Math.Sin;
        const float step = 0.001f;
        for (float i = beg; i >= end; i -= step)
        {
            float x = (float)(cos(i) * radius);
            float xl = (float)(cos(i) * (radius - thickness));
            float y = (float)(sin(i) * radius);
            float yl = (float)(sin(i) * (radius - thickness));

            float x2 = (float)(cos(i - step) * radius);
            float x2l = (float)(cos(i - step) * (radius - thickness));
            float y2 = (float)(sin(i - step) * radius);
            float y2l = (float)(sin(i - step) * (radius - thickness));

            ret.Add(x); ret.Add(y); ret.Add(z);

            ret.Add(xl); ret.Add(yl); ret.Add(z);
            ret.Add(x2l); ret.Add(y2l); ret.Add(z);

            ret.Add(x); ret.Add(y); ret.Add(z);
            ret.Add(x2); ret.Add(y2); ret.Add(z);
            ret.Add(x2l); ret.Add(y2l); ret.Add(z);
        }

        ushort[] indices = new ushort[ret.Count / 3];
        for(int i = 0; i < indices.Length; i++)
            indices[i] = (ushort)i;
        return (ret.ToArray(), indices);
    }

    public void Draw(ITimerSource source, SpeedToolUISettings config)
    {
        SecondsColor = config.SecondsClockTimerColor;
        MinutesColor = config.MinutesClockTimerColor;
        HoursColor = config.HoursClockTimerColor;
        
        gl.Enable(GLEnum.PolygonSmooth);
        s.DrawWith(() => DrawBuffers(source));

        // TODO: The notches code is wrong somehow, fix later
        /*s.DrawWith(() =>
        {
            var mat = GetOrtho(-1.0f, 1.0f, -1.0f, 1.0f, 0.0f, 100.0f);
            s.SetUniform("projection", mat);
            s.SetUniform("clr", new Vector3D<float>(1.0f, 1.0f, 1.0f));
            var coor = s.GetAttribLocation("coordinates");
            notches.DrawAll((uint)coor);
        });*/
    }

    /// <summary>
    /// Gets appropriate bar progress for timer source.
    /// </summary>
    /// <param name="source"></param>
    /// <returns>Bar progress in [0; 1] range. If the source isn't started, return (1, 1, 1).</returns>
    private (float sec, float min, float hrs) GetBarProgressFromSource(ITimerSource source)
    {
        if(source.CurrentState == TimerState.NoState)
            return (60, 60, 12);

        return ((float)source.CurrentTime.FloatSeconds() / 60.0f,
                (float)source.CurrentTime.FloatMinutes() / 60.0f,
                (float)source.CurrentTime.FloatHours() / 12.0f);
    }

    private void DrawBuffers(ITimerSource source)
    {
        var mat = GetOrtho(-1.0f, 1.0f, -1.0f, 1.0f, 0.0f, 100.0f);
        s.SetUniform("projection", mat);
        s.SetUniform("clr", new Vector3D<float>(SecondsColor.X, SecondsColor.Y, SecondsColor.Z));
        var coor = s.GetAttribLocation("coordinates");
        var progress = GetBarProgressFromSource(source);
        objects[0].Draw((int)(objects[0].Length * progress.sec), (uint)coor);

        s.SetUniform("clr", new Vector3D<float>(MinutesColor.X, MinutesColor.Y, MinutesColor.Z));
        objects[1].Draw((int)(objects[1].Length * progress.min), (uint)coor);

        s.SetUniform("clr", new Vector3D<float>(HoursColor.X, HoursColor.Y, HoursColor.Z));
        objects[2].Draw((int)(objects[2].Length * progress.hrs), (uint)coor);
    }

    static Matrix4X4<float> GetOrtho(float left, float right, float bottom, float top, float znear, float zfar)
    {
        return new Matrix4X4<float>(new Vector4D<float>(2.0f / (right - left), 0.0f, 0.0f, 0.0f),
                                    new Vector4D<float>(0.0f, 2.0f / (top - bottom), 0.0f, 0.0f),
                                    new Vector4D<float>(0.0f, 0.0f, 1.0f / (zfar - znear), 0.0f),
                                    new Vector4D<float>((left + right) / (left - right), (top + bottom) / (bottom - top), -znear / (znear - zfar), 1.0f));
    }

    public void Dispose()
    {
        s.Dispose();
        foreach(var obj in objects)
            obj.Dispose();
    }

    const string fragCode =@"
        uniform vec3 clr;
        void main(void) {
            gl_FragColor = vec4(clr, 1.0);
        }";

    const string vertCode =@"
        attribute vec3 coordinates;
        uniform mat4 projection;
        void main(void) {
        gl_Position = projection * vec4(coordinates, 1.0);
        }";

    GL gl;
    private STShader s;
    private DrawObject[] objects;

    private DrawObject notches;
}
