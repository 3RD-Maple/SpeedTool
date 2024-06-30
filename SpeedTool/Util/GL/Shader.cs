namespace SpeedTool.Util.GL;

using Silk.NET.Maths;
using Silk.NET.OpenGL;

sealed class Shader : IDisposable
{
    public Shader(string vertexSource, string fragmentSource, GL gl)
    {
        this.gl = gl;

        vertex = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertex, vertexSource);
        gl.CompileShader(vertex);
        //std::cout << get_log(vertex) << std::endl;

        fragment = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fragment, fragmentSource);
        gl.CompileShader(fragment);
        //std::cout << get_log(fragment) << std::endl;

        program = gl.CreateProgram();
        gl.AttachShader(program, vertex);
        gl.AttachShader(program, fragment);
        gl.LinkProgram(program);
        gl.UseProgram(program);

        uniforms = new Dictionary<string, int>();
    }

    public int GetAttribLocation(string name)
    {
        return gl.GetAttribLocation(program, name);
    }

    public void SetUniform(string key, Vector4D<float> value)
    {
        gl.Uniform4(ReadUniformLoc(key), value.X, value.Y, value.Z, value.W);
    }

    public void SetUniform(string key, Vector3D<float> value)
    {
        gl.Uniform3(ReadUniformLoc(key), value.X, value.Y, value.Z);
    }

    public void SetUniform(string key, Matrix4X4<float> value)
    {
        float[] dub = new float[4*4];
        for(int i = 0; i < 16; i++)
            dub[i] = value[i % 4][i / 4];
        gl.UniformMatrix4(ReadUniformLoc(key), false, dub.AsSpan());
    }

    public void DrawWith(Action a)
    {
        gl.UseProgram(program);
        a();
        gl.UseProgram(0);
    }

    public void Dispose()
    {
        gl.DeleteProgram(program);
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);
    }

    private int ReadUniformLoc(string name)
    {
        if(uniforms.ContainsKey(name))
            return uniforms[name];

        var loc = gl.GetUniformLocation(program, name);
        if(loc < 0)
            throw new Exception("Uniform not found");

        uniforms[name] = loc;
        return loc;
    }

    private Dictionary<string, int> uniforms;

    private GL gl;

    private uint program;
    private uint vertex;
    private uint fragment;
}