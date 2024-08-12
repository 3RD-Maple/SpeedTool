using Silk.NET.OpenGL;
using StbImageSharp;
using System.Numerics;

namespace SpeedTool.Platform;

public class Images
{
    public Images(GL gl)
    {
        this.gl = gl;
    }

    public void LoadImage(string name, ImageResult image)
    {
        // TODO: having 1 texure per image is wasteful, do something about it (later) :)
        var tex = gl.GenTexture();
        gl.Enable(EnableCap.Texture2D);
        gl.BindTexture(GLEnum.Texture2D, tex);

        unsafe
        {
            fixed(byte* data = image.Data)
                gl.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
        }

        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
        gl.GenerateMipmap(TextureTarget.Texture2D);
        gl.BindTexture(GLEnum.Texture2D, 0);
        textures[name] = new Image()
        {
            Handle = new IntPtr(tex),
            Sizes = new Vector2(image.Width, image.Height)
        };
    }

    public Image this[string name]
    {
        get
        {
            return textures[name];
        }
    }

    private Dictionary<string, Image> textures = new();

    private GL gl;
}
