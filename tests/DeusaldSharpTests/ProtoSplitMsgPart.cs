// Third partial declaration of SplitMsg (see ProtoSecurityTests.cs): a message split across files must be generated once.

using DeusaldSharp;

namespace DeusaldSharpTests
{
    public partial class SplitMsg
    {
        [ProtoField(3)] public int C;
    }
}
