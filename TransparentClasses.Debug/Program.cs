using TransparentClasses;

var t = new TransparentTest()
{
    Object = new()
};

t.PublicMethod("1", 2);
t.PrivateVoidMethodNoParams();
t.PrivateIntMethodNoParams();
t.PrivateVoidMethodWithParams(10, "some string", new SomeObject { Value = 90 });
t.PrivateStringMethodWithParams(20, "some string 2", new SomeObject { Value = 800 });
t.PrivateVoidGenericMethodWithParams<System.Threading.Tasks.Dataflow.ActionBlock<int>>(30, "some string 3", new SomeObject { Value = 2 });
t.PrivateStringGeneric2MethodWithParams<int, SomeObject>(40, "some string 4", new SomeObject { Value = -789 });


public partial class TransparentTest : TransparentObject<TestClass1>
{ }

public class TestClass1
{
    public void PublicMethod(string a, int b) 
    {
        Console.WriteLine(nameof(PublicMethod));
    }

    private void PrivateVoidMethodNoParams() 
    { 
        Console.WriteLine(nameof(PrivateVoidMethodNoParams));
    }

    private int PrivateIntMethodNoParams()
    {
        Console.WriteLine(nameof(PrivateIntMethodNoParams));
        return 111;
    }

    private void PrivateVoidMethodWithParams(int a, string b, SomeObject c)
    {
        Console.WriteLine($"{nameof(PrivateVoidMethodWithParams)}; {a}, {b}, {c}");
    }

    private string PrivateStringMethodWithParams(int a, string b, SomeObject c)
    {
        var str = $"{nameof(PrivateStringMethodWithParams)}; {a}, {b}, {c}";
        Console.WriteLine(str);
        return str;
    }

    private void PrivateVoidGenericMethodWithParams<T>(int a, string b, SomeObject c)
    {
        Console.WriteLine($"{nameof(PrivateVoidGenericMethodWithParams)} {typeof(T)}; {a}, {b}, {c}");
    }

    private string PrivateStringGeneric2MethodWithParams<T1, T2>(int a, string b, SomeObject c)
    {
        var str = $"{nameof(PrivateStringGeneric2MethodWithParams)} {typeof(T1)} {typeof(T2)}; {a}, {b}, {c}";
        Console.WriteLine(str);
        return str;
    }
}

public record SomeObject
{
    public int Value { get; set; }
}


