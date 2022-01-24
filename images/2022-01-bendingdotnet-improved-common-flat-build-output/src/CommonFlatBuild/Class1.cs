namespace CommonFlatBuild;

public class Class1
{
    public void Test()
    {
        using var writer = new StringWriter();
        NewMethod(writer);
        // Comment
    }

    private static void NewMethod(StringWriter writer)
    {
        writer.Write("TEST");
    }
}
