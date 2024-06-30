namespace SpeedTool.Util.GL;

using Silk.NET.OpenGL;

sealed class DrawObject : IDisposable
{
    public DrawObject(GL gl, float[] vertex, ushort[] index)
    {
        this.gl = gl;
        vao = gl.GenVertexArray();
        indices = gl.CreateBuffer();
        vbo = gl.CreateBuffer();

        gl.BindVertexArray(vao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertex.AsSpan(), BufferUsageARB.StaticDraw);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, this.indices);
        gl.BufferData<ushort>(BufferTargetARB.ElementArrayBuffer, index.AsSpan(), BufferUsageARB.StaticDraw);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        Length = index.Length;
    }

    public void Dispose()
    {
        gl.DeleteBuffer(vbo);
        gl.DeleteBuffer(indices);
        gl.DeleteVertexArray(vao);
    }

    public void Draw(int elements, uint unif)
    {
        Bind();
        gl.VertexAttribPointer(unif, 3, GLEnum.Float, false, 0, 0);
        gl.EnableVertexAttribArray(unif);

        ReadOnlySpan<ushort> s = new ReadOnlySpan<ushort>(); // This is just to shut up the compiler.
        gl.DrawElements(GLEnum.Triangles, (uint)elements, GLEnum.UnsignedShort, s);
        gl.DisableVertexAttribArray(unif);
        Unbind();
    }

    public int Length { get; private set; }

    private void Bind()
    {
        gl.BindVertexArray(vao);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indices);
    }

    private void Unbind()
    {
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        gl.BindVertexArray(0);
    }

    private GL gl;
    private uint vao;
    private uint indices;
    private uint vbo;
}
