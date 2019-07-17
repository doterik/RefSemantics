using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace RefSemantics
{
    public class Span
    {
        private const string myString = "Hello world";
        private static readonly int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        [Fact]
        public void String_as_read_only_span()
        {
            var span = myString.AsSpan(); // ReadOnlySpan<char>
            AssertEqual(span.Length, "Hello world".Length);
            AssertTrue(span.Equals("Hello world", StringComparison.InvariantCultureIgnoreCase));
            AssertTrue(span.StartsWith("Hello"));
        }

        [Fact]
        public void Substring_as_read_only_slice()
        {
            //var span = myString.AsSpan(); // ReadOnlySpan<char>
            //var slice = span.Slice(6);    // ReadOnlySpan<char>
            var slice = myString.AsSpan().Slice(6); // ReadOnlySpan<char>
            AssertEqual("world".Length, slice.Length);
            AssertTrue(slice.Equals("world", StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public void Array_as_mutable_span_and_slice()
        {
            //Span<int> numberSpan = numbers; // !!
            //var numberSpan = numbers.AsSpan();
            //var numberSlice = numberSpan.Slice(3, 5); // Span<int>
            var numberSlice = numbers.AsSpan().Slice(3, 5); // Span<int>
            numberSlice[2] = 100;

            AssertEqual(1, numbers[0]);
            AssertEqual(2, numbers[1]);
            AssertEqual(3, numbers[2]);
            AssertEqual(4, numbers[3]);
            AssertEqual(5, numbers[4]);
            AssertEqual(100, numbers[5]);
            AssertEqual(7, numbers[6]);
            AssertEqual(8, numbers[7]);
            AssertEqual(9, numbers[8]);
            AssertEqual(10, numbers[9]);
        }

        [Fact]
        public void Allocation_free()
        {
            var totalMemory = GC.GetTotalMemory(true);
            AssertTrue(GC.TryStartNoGCRegion(1000));

            String_as_read_only_span();
            Substring_as_read_only_slice();
            Array_as_mutable_span_and_slice();

            // Crude, not a guarantee that nothing was allocated.
            GC.EndNoGCRegion();
            Assert.Equal(0, GC.GetTotalMemory(false) - totalMemory);
        }

        #region AssertEqual implementation details
        private static readonly IEqualityComparer<int> intEqualityComparer = EqualityComparer<int>.Default;

        static Span()
        {
            // We need to call these once, or we get allocations in the test.
            // I don't really know why, but I'm guessing it's something to do with JIT?
            AssertEqual(1, 1);
            AssertTrue(true);
            var xx = new Span();
            xx.String_as_read_only_span();
            xx.Substring_as_read_only_slice();
            xx.Array_as_mutable_span_and_slice();
        }

        private static void AssertEqual(int expected, int actual) => Assert.Equal(expected, actual, intEqualityComparer);

        private static void AssertTrue(bool condition)
        {
            if (!condition) throw new TrueException(null, false);
        }
        #endregion
    }
}