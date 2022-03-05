using AdventOfCode.Days.Tools.Day19;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AoTest.AoC2021
{
    [TestClass]
    public class Day19Test
    {
        static Point3 basePoint = new Point3(1, 2, 3);

        [TestMethod]
        public void TestPointComparison()
        {
            Assert.AreEqual(basePoint, basePoint.Clone());
            Assert.AreEqual(basePoint, new Point3(1, 2, 3));
            Assert.AreNotEqual(basePoint, new Point3(3, 2, 1));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPossibleRotations), DynamicDataSourceType.Method)]
        public void Point3RotationTest(int rotationStep, Point3 expected)
        {
            
            Assert.AreEqual(expected, basePoint.Rotate(rotationStep));
        }

        public static IEnumerable<object[]> GetPossibleRotations()
        {
            yield return new object[] {0, basePoint };
            yield return new object[] { 1, new Point3( -2, 1, 3) };
            yield return new object[] { 2, new Point3( -1, -2, 3) };
            yield return new object[] { 3, new Point3( 2, -1, 3) };
            yield return new object[] { 4, new Point3( 1, -3, 2) };
            yield return new object[] { 5, new Point3( 3, 1, 2) };
            yield return new object[] { 6, new Point3( -1, 3, 2) };
            yield return new object[] { 7, new Point3( -3, -1, 2) };
            yield return new object[] { 8, new Point3( 1, -2, -3) };
            yield return new object[] { 9, new Point3( 2, 1, -3) };
            yield return new object[] { 10, new Point3( -1, 2, -3) };
            yield return new object[] { 11, new Point3( -2, -1, -3) };
            yield return new object[] { 12, new Point3( 1, 3, -2) };
            yield return new object[] { 13, new Point3( -3, 1, -2) };
            yield return new object[] { 14, new Point3( -1, -3, -2) };
            yield return new object[] { 15, new Point3( 3, -1, -2) };
            yield return new object[] { 16, new Point3( 3, 2, -1) };
            yield return new object[] { 17, new Point3( -2, 3, -1) };
            yield return new object[] { 18, new Point3( -3, -2, -1) };
            yield return new object[] { 19, new Point3( 2, -3, -1) };
            yield return new object[] { 20, new Point3( -3, 2, 1) };
            yield return new object[] { 21, new Point3( -2, -3, 1) };
            yield return new object[] { 22, new Point3( 3, -2, 1) };
            yield return new object[] { 23, new Point3( 2, 3, 1) };
        }
    }
}
