//#define CompileError

using Xunit;

namespace RefSemantics
{
    public class InParameters
    {
        private struct MutablePoint
        {
            public float X;
            public float Y;

            public MutablePoint(float x, float y)
            {
                X = x;
                Y = y;
            }

            public void TranslateInPlace(float dx, float dy)
            {
                X += dx;
                Y += dy;
            }
        }

        [Fact]
        public void Method_can_mutate_own_state()
        {
            var p = new MutablePoint(10, 10);
            p.TranslateInPlace(20, 0);
            Assert.Equal(30, p.X);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        private static void DoSomething(in MutablePoint point)
        {
#if CompileError
            point.X = 23; /* Compile error! */
#endif
            point.TranslateInPlace(10, 10);
        }

        [Fact]
        public void Call_method_on_in_parameter_Dummy()
        {
            var p = new MutablePoint(10, 10);
            DoSomething(p);
        }

        private static void DoTranslate(in MutablePoint p, int dx, int dy)
        {
            // in parameter, called on a defensive copy.
            p.TranslateInPlace(dx, dy);

            // Translate didn't work!
            Assert.Equal(10, p.X);
        }

        [Fact]
        public void Call_method_on_in_parameter_makes_copy()
        {
            var p = new MutablePoint(10, 10);
            DoTranslate(p, 20, 0);
        }
    }
}