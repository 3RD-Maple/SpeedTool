using System.Runtime.InteropServices;

namespace SpeedTool.Platform.Windows;

// This isn't the most correct representation, but oh well
[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
public class OPENFILENAME 
{
    public int      lStructSize = Marshal.SizeOf<OPENFILENAME>();
    public IntPtr   hwndOwner = IntPtr.Zero; 
    public IntPtr   hInstance = IntPtr.Zero;

    public string?   lpstrFilter = null;
    public string?   lpstrCustomFilter = null;
    public int      nMaxCustFilter = 0;
    public int      nFilterIndex = 0;

    public string   lpstrFile = new string(new char[256]);
    public int      nMaxFile = 0;

    public string   lpstrFileTitle = new string(new char[256]);
    public int      nMaxFileTitle = 0;

    public string?   lpstrInitialDir = null;

    public string?   lpstrTitle = null;   

    public int      Flags = 0; 
    public short    nFileOffset = 0;
    public short    nFileExtension = 0;

    public string?   lpstrDefExt = null; 

    public IntPtr   lCustData = IntPtr.Zero;  
    public IntPtr   lpfnHook = IntPtr.Zero;  

    public string?   lpTemplateName = null; 

    // Those are in the docs, but it stops working if I define them. Not sure why
    /*public IntPtr    lpEditInfo;
    public string?   lpstrPrompt;*/

    public IntPtr   pvReserved = IntPtr.Zero; 
    public int      dwReserved = 0;
    public int      FlagsEx = 0;
}
