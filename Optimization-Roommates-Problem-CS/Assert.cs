using System.Diagnostics;

namespace Optimization_Roommates_Problem_CS;

public static class Assert
{
    [Conditional("DEBUG")]
    public static void IsTrue(bool condition, string message = "")
    {
        if (!condition)
            throw new AssertionFailedException(message);
    }
}

public class AssertionFailedException : Exception
{
    public AssertionFailedException(string message) : base(message)
    { }
}