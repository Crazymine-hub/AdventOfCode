using System;
using System.Reflection;
using Eto.Containers;
using Eto.Drawing;

namespace AdventOfCode.Tools.Visualization.Controls;

public class MovableImageView : DragZoomImageView
{
    private const string PeopertyName = "_transform";
    FieldInfo transformField;
    public MovableImageView()
    {

        transformField = typeof(MovableImageView)?.BaseType?.GetField(PeopertyName, BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new NullReferenceException($"Unable to get private field {PeopertyName} of base class");
    }

    /// <summary>
    /// Moves a pixel to a space on screen
    /// </summary>
    /// <param name="pixel">The coordinates of the Pixel</param>
    /// <param name="moveto">The Space on Screen</param>
    /// <example>
    /// <code>MovePixel(new PointF(0, 0), (Point)Size/2)</code>
    /// moves the top left corner of the image to the center.
    /// </example>
    public void MovePixel(PointF pixel, PointF moveto)
    {

        var baseTransform = (IMatrix)(transformField?.GetValue(this) ??
            throw new TargetException($"Unable to get value of field {transformField}"));
        //start = _transform.Inverse().TransformPoint(start);
        moveto = baseTransform.Inverse().TransformPoint(moveto);

        baseTransform.Translate(moveto - pixel);

        this.Invalidate();
    }
}