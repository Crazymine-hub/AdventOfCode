using AdventOfCode.Tools.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days.Tools.Day19
{
    internal struct IntersectResult
    {
        public SensorData RootSensor { get; }
        public SensorData TargetSensor { get; }
        public Point3 RootPoint { get; }
        public Point3 TargetPoint { get; }
        public int TargetRotation { get; }

        public IntersectResult(SensorData rootSensor, Point3 rootPoint, SensorData targetSensor, Point3 targetPoint, int targetRotation)
        {
            RootPoint = rootPoint;
            TargetPoint = targetPoint;
            TargetRotation = targetRotation;
            TargetSensor = targetSensor;
            RootSensor = rootSensor;
        }

        public SensorData GetOther(SensorData current)
        {
            if (current.Equals(RootSensor))
                return TargetSensor;
            if (current.Equals(TargetSensor))
                return RootSensor;
            throw new ArgumentException($"Sensor {current} not in this intersection");
        }

        public IntersectResult Rotate(int rotation) =>
            new IntersectResult(
                RootSensor.Rotate(rotation),
                RootPoint.Rotate(rotation),
                TargetSensor.Rotate(rotation),
                TargetPoint.Rotate(rotation),
                TargetRotation);

        public override string ToString() => $"{RootSensor.Name} X {TargetSensor.Name}";
    }
}
