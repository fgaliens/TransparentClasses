using System.Reflection;
using TransparentClasses;
using T3 = TestClass3;

var t = new TransparentTest()
{
    Object = new()
};

//t.PublicMethod("", 234);

Console.WriteLine(t.);



//[TransparentObject<TestClass1>]


public partial class TransparentTest : TransparentObject<TestClass2>
{
}

public class TestClass1
{
    public void PublicMethod(string a, int b) { }

    private int PrivateMethod() { return 10; }

    private string PrivMeth2(int abc, string value)
    {
        return $"{abc} {value}";
    }

    private string PrGenMeth<T>(int a)
    {
        return $"{typeof(T)} {a + 1}";
    }
}

public class TestClass2
{
    private int _prField;

    private string? _prProp { get; set; }

    public virtual void PublicMethod23(int count)
    {
        var c = 21;

        if (count > 0)
        {
            c += 67;
        }
        else
        {
            return;
        }

        var t = new TestClass1();
        t.PublicMethod("", 34);
    }

    private void PrivateMethod34() { }

    private int PrivateMethodThatReturns()
    {
        return 0;
    }

    void PrivateGenericMethod<T>() { }
}

public class TestClass3
{
    public void PublicMethod23() { }

    private void PrivateMethod34() { }
}

