//#define CompileError

using Xunit;

namespace RefSemantics
{
    public class ReadonlyStruct
    {
        private readonly struct Point
        {
            // Compiler enforces readonly here.
            public readonly float X;
            public readonly float Y;

            public Point(float x, float y)
            {
                X = x;
                Y = y;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "<Pending>")]
            public void TranslateInPlace(float dx, float dy)
            {
#if CompileError
                X += dx; /* Compile error - immutable! */
                Y += dy;
#endif
            }

            private static readonly Point origin = new Point(0, 0);

            public static ref readonly Point Origin => ref origin;
        }

        [Fact]
        public void Non_ref_assignment_makes_copy()
        {
            // This is a copy, but we can't do anything dangerous anyway. It's immutable!
            var origin = Point.Origin;

#if CompileError
            origin.X = 42; /* Compile error! */
#endif
            Assert.Equal(0, origin.X);
        }

        [Fact]
        public void TestRefReadonlyReturn()
        {
            ref readonly var origin = ref Point.Origin;

#if CompileError
            origin.X = 42; /* Compile error! */
#endif

            // Method calls are on original value, but the method can't do anything dangerous. It's immutable!
            origin.TranslateInPlace(42, 0);

            Assert.Equal(0, origin.X);
        }
    }
}