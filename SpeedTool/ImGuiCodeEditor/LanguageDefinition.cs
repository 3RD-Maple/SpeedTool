namespace ImGuiCodeEditor;

using Silk.NET.Vulkan;
using Identifiers = Dictionary<string, Identifier>; 
using Keywords = List<string>;

public class LanguageDefinition
{
    //typedef std::pair<std::string, PaletteIndex> TokenRegexString;
    //List<(string, PaletteIndex)> TokenRegexStrings;
    //typedef bool(*TokenizeCallback)(const char * in_begin, const char * in_end, const char *& out_begin, const char *& out_end, PaletteIndex & paletteIndex);

    public delegate bool TokenizeCallback(string line, ref int token_begin, ref int token_end, ref PaletteIndex token_color);

    public string mName = "";
    public Keywords mKeywords = new();
    public Identifiers mIdentifiers = new();
    public Identifiers mPreprocIdentifiers = new();
    public string mCommentStart = "", mCommentEnd = "", mSingleLineComment = "";
    public char mPreprocChar = '#';
    public bool mAutoIndentation = true;

    public TokenizeCallback? mTokenize = null;

    public List<(string, PaletteIndex)> mTokenRegexStrings = new();

    public bool mCaseSensitive = true;

    public LanguageDefinition() { }

    /*public static LanguageDefinition CPlusPlus();
    public static LanguageDefinition HLSL();
    public static LanguageDefinition GLSL();
    public static LanguageDefinition C();
    public static LanguageDefinition SQL();
    public static LanguageDefinition AngelScript();*/
    public static LanguageDefinition Lua
    {
        get
        {
            LanguageDefinition langDef = new();

            string[] keywords = [
                "and", "break", "do", "", "else", "elseif", "end", "false", "for", "function", "if", "in", "", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"
            ];

            foreach(var k in keywords)
                langDef.mKeywords.Add(k);

            string[] identifiers = [
                "assert", "collectgarbage", "dofile", "error", "getmetatable", "ipairs", "loadfile", "load", "loadstring",  "next",  "pairs",  "pcall",  "print",  "rawequal",  "rawlen",  "rawget",  "rawset",
                "select",  "setmetatable",  "tonumber",  "tostring",  "type",  "xpcall",  "_G",  "_VERSION","arshift", "band", "bnot", "bor", "bxor", "btest", "extract", "lrotate", "lshift", "replace",
                "rrotate", "rshift", "create", "resume", "running", "status", "wrap", "yield", "isyieldable", "debug","getuservalue", "gethook", "getinfo", "getlocal", "getregistry", "getmetatable",
                "getupvalue", "upvaluejoin", "upvalueid", "setuservalue", "sethook", "setlocal", "setmetatable", "setupvalue", "traceback", "close", "flush", "input", "lines", "open", "output", "popen",
                "read", "tmpfile", "type", "write", "close", "flush", "lines", "read", "seek", "setvbuf", "write", "__gc", "__tostring", "abs", "acos", "asin", "atan", "ceil", "cos", "deg", "exp", "tointeger",
                "floor", "fmod", "ult", "log", "max", "min", "modf", "rad", "random", "randomseed", "sin", "sqrt", "string", "tan", "type", "atan2", "cosh", "sinh", "tanh",
                "pow", "frexp", "ldexp", "log10", "pi", "huge", "maxinteger", "mininteger", "loadlib", "searchpath", "seeall", "preload", "cpath", "path", "searchers", "loaded", "module", "require", "clock",
                "date", "difftime", "execute", "exit", "getenv", "remove", "rename", "setlocale", "time", "tmpname", "byte", "char", "dump", "find", "format", "gmatch", "gsub", "len", "lower", "match", "rep",
                "reverse", "sub", "upper", "pack", "packsize", "unpack", "concat", "maxn", "insert", "pack", "unpack", "remove", "move", "sort", "offset", "codepoint", "char", "len", "codes", "charpattern",
                "coroutine", "table", "io", "os", "string", "utf8", "bit32", "math", "debug", "package"
            ];
            foreach (var k in identifiers)
            {
                Identifier id = new();
                id.mDeclaration = "Built-in function";
                langDef.mIdentifiers[k] = id;
            }

            langDef.mTokenRegexStrings.Add(new("L?\\\"(\\\\.|[^\\\"])*\\\"", PaletteIndex.String));
            langDef.mTokenRegexStrings.Add(new("\\\'[^\\\']*\\\'", PaletteIndex.String));
            langDef.mTokenRegexStrings.Add(new("0[xX][0-9a-fA-F]+[uU]?[lL]?[lL]?", PaletteIndex.Number));
            langDef.mTokenRegexStrings.Add(new("[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)([eE][+-]?[0-9]+)?[fF]?", PaletteIndex.Number));
            langDef.mTokenRegexStrings.Add(new("[+-]?[0-9]+[Uu]?[lL]?[lL]?", PaletteIndex.Number));
            langDef.mTokenRegexStrings.Add(new("[a-zA-Z_][a-zA-Z0-9_]*", PaletteIndex.Identifier));
            langDef.mTokenRegexStrings.Add(new("[\\[\\]\\{\\}\\!\\%\\^\\&\\*\\(\\)\\-\\+\\=\\~\\|\\<\\>\\?\\/\\;\\,\\.]", PaletteIndex.Punctuation));

            langDef.mCommentStart = "--[[";
            langDef.mCommentEnd = "]]";
            langDef.mSingleLineComment = "--";

            langDef.mCaseSensitive = true;
            langDef.mAutoIndentation = false;

            langDef.mName = "Lua";

            return langDef;
        }
    }
}