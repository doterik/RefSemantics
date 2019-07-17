using Xunit;

namespace RefSemantics
{
    public class RefReturnAndRefLocal
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
        }

        private class Enemy
        {
            private Point location;
            public Point Location => location;
            public Enemy(Point location) => this.location = location;
            public ref Point GetLocationByRef() => ref location;
        }

        [Fact]
        public void Struct_return_value_is_a_copy()
        {
            var enemy = new Enemy(new Point(10, 10));

            // This is a copy!
            var location = enemy.Location;
            location.X = 12;

            Assert.Equal(12, location.X);
            Assert.Equal(10, enemy.Location.X);
        }

        [Fact]
        public void Ref_return_value_is_a_copy()
        {
            var enemy = new Enemy(new Point(10, 10));

            // This is a copy, even returned by ref!
            var copy = enemy.GetLocationByRef();
            copy.X = 12;

            Assert.Equal(12, copy.X);
            Assert.Equal(10, enemy.GetLocationByRef().X);
        }

        [Fact]
        public void Ref_return_to_var_ref_no_copy()
        {
            var enemy = new Enemy(new Point(10, 10));

            // Reference, not copy.
            ref var location = ref enemy.GetLocationByRef();
            location.X = 12;

            Assert.Equal(12, location.X);
            Assert.Equal(12, enemy.Location.X);
        }

        [Fact]
        public void Use_method_as_left_hand_side()
        {
            var enemy = new Enemy(new Point(10, 10));

            enemy.GetLocationByRef() = new Point(42, 42);

            Assert.Equal(42, enemy.Location.X);
        }
    }
}