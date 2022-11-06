
// ---------------------------------------------------------------------------------------------------------------------
// Properties: 
// - Build Action: Content
// - Copy to output directory: Copy if newer
// ---------------------------------------------------------------------------------------------------------------------

using DynamicRun.Sources.Interfaces;

namespace DynamicRun.Sources;

public class MyMath : IMath
{
   public int A { get; set; }
   public int  B { get; set; }

   public MyMath()
   {
      
   }
   
   public MyMath(int a, int b)
   {
      A = a;
      B = b;
   }

   public int Sum()
   {
      return A + B;
   }
}