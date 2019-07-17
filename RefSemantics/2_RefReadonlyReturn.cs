//#define CompileError

using Xunit;

namespace RefSemantics
{
    public class RefReadonlyReturn
    {
        private struct Point
        {
            public float X;
            public float Y;

            public Point(float x, float y)
            {
                X = x;
                Y = y;
            }

            public void TranslateInPlace(float dx, float dy)
            {
                X += dx;
                Y += dy;
            }

            private static Point origin = new Point(0, 0);
            public static ref Point Origin => ref origin;

            private static readonly Point readonlyOrigin = new Point(0, 0);
            public static ref readonly Point ReadonlyOrigin => ref readonlyOrigin;
        }

        [Fact]
        public void Non_ref_variable_receives_copy()
        {
            // Receives copy, not reference. This won't modify 'Point.Origin'.
            var copy = Point.Origin;
            copy.X = 42;

            Assert.Equal(0, Point.Origin.X);
            Assert.Equal(42, copy.X);
        }

        [Fact]
        public void Non_readonly_ref_variable_modifies_shared_state()
        {
            // Non readonly reference! Modifies shared state.
            ref var origin = ref Point.Origin;
            origin.X = 42;

            Assert.Equal(42, Point.Origin.X);
        }

        [Fact]
        public void Ref_readonly_cannot_be_modified()
        {
            ref readonly var origin = ref Point.ReadonlyOrigin;
#if CompileError
            origin.X = 42; /* Compile error! */
#endif
            Assert.Equal(0, origin.X);
            Assert.Equal(0, Point.ReadonlyOrigin.X);
        }

        [Fact]
        public void Ref_readonly_makes_copy_for_method_call()
        {
            ref readonly var origin = ref Point.ReadonlyOrigin;

            // Method call happens on defensive copy.
            origin.TranslateInPlace(42, 0);

            Assert.Equal(0, Point.ReadonlyOrigin.X);
            Assert.Equal(0, origin.X);
        }

        // Reset everything between tests. Ignore me! ☺️
        public RefReadonlyReturn() => Point.Origin.X = 0;
    }
}