using System.Runtime.CompilerServices;

var t1 = new Thread(T1);
var wh = new ManualResetEvent(false);
t1.Start(wh);

Thread.Sleep(100);

wh.Set();

Thread.Sleep(20000);

[MethodImpl(MethodImplOptions.NoInlining)]
void T1(object? obj)
{
    M1(obj);
}

[MethodImpl(MethodImplOptions.NoInlining)]
void M1(object? obj)
{
    M2(obj);
}

[MethodImpl(MethodImplOptions.NoInlining)]
void M2(object? obj)
{
    M3(obj);
}

[MethodImpl(MethodImplOptions.NoInlining)]
void M3(object? obj)
{
    (obj as ManualResetEvent)?.WaitOne();
    throw new ArgumentException(obj?.ToString());
}