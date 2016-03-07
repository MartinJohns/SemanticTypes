using System;
using System.Diagnostics;

namespace SemanticTypes
{
    public static class Performance
    {
        public static void Run()
        {
            const int iterations = 100000000; // OVER 9000! (10 million)

            Normal(5);
            Semantic(new PaxId(5));

            var sw = new Stopwatch();

            sw.Start();
            for (var i = 0; i < iterations; ++i)
                Normal(5);
            sw.Stop();
            Console.WriteLine($"Normal: {sw.ElapsedMilliseconds}ms");
            sw.Reset();

            sw.Start();
            for (var i = 0; i < iterations; ++i)
                Semantic(new PaxId(5));
            sw.Stop();
            Console.WriteLine($"Normal: {sw.ElapsedMilliseconds}ms");
            sw.Reset();

            Console.ReadLine();
        }

        public static void Normal(int paxId)
        {
            if (paxId < 0)
                throw new ArgumentException("Invalid pax id.", nameof(paxId));
        }

        public static void Semantic(PaxId paxId)
        {
            if (paxId == PaxId.None)
                throw new ArgumentException("Invalid pax id.", nameof(paxId));
        }

        /* IL
           Copied from a LinqPad query.

           Run:
           IL_0000:  nop         
           IL_0001:  ldc.i4.5    
           IL_0002:  call        UserQuery.Normal
           IL_0007:  nop         
           IL_0008:  ldc.i4.5    
           IL_0009:  newobj      UserQuery+PaxId..ctor
           IL_000E:  call        UserQuery.Semantic
           IL_0013:  nop         
           IL_0014:  ret         
      
           Normal:
           IL_0000:  nop         
           IL_0001:  ldarg.0     
           IL_0002:  ldc.i4.0    
           IL_0003:  clt         
           IL_0005:  stloc.0     
           IL_0006:  ldloc.0     
           IL_0007:  brfalse.s   IL_0019
           IL_0009:  ldstr       "Invalid pax id."
           IL_000E:  ldstr       "paxId"
           IL_0013:  newobj      System.ArgumentException..ctor
           IL_0018:  throw       
           IL_0019:  ret         
           
           Semantic:
           IL_0000:  nop         
           IL_0001:  ldarg.0     
           IL_0002:  ldsfld      UserQuery+PaxId.None
           IL_0007:  call        UserQuery+PaxId.op_Equality
           IL_000C:  stloc.0     
           IL_000D:  ldloc.0     
           IL_000E:  brfalse.s   IL_0020
           IL_0010:  ldstr       "Invalid pax id."
           IL_0015:  ldstr       "paxId"
           IL_001A:  newobj      System.ArgumentException..ctor
           IL_001F:  throw       
           IL_0020:  ret     
        
        */
    }
}
