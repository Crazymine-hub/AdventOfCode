using System.Diagnostics;

namespace AdventOfCode.Days.Tools.Day24
{
    [DebuggerDisplay("Step: x:{XOffset} y:{YOffset} z:{ZLimit} level:{InEncodeLevel}")]
    internal struct ProgramStep
    {
        public int XOffset { get; }
        public int YOffset { get; }
        public bool WantsEncode { get; }
        public int InEncodeLevel { get; }
        public int ZLimit { get; }

        public ProgramStep(int xOffset, int yOffset, int zLimit, int inEncodeLevel)
        {
            this.XOffset = xOffset;
            this.YOffset = yOffset;
            this.ZLimit = zLimit;
            WantsEncode = zLimit == 1;
            InEncodeLevel = inEncodeLevel;
        }

        public override bool Equals(object obj)
        {
            return obj is ProgramStep other &&
                   XOffset == other.XOffset &&
                   YOffset == other.YOffset &&
                   WantsEncode == other.WantsEncode &&
                   InEncodeLevel == other.InEncodeLevel;
        }

        public override int GetHashCode()
        {
            int hashCode = 610136175;
            hashCode = hashCode * -1521134295 + XOffset.GetHashCode();
            hashCode = hashCode * -1521134295 + YOffset.GetHashCode();
            hashCode = hashCode * -1521134295 + WantsEncode.GetHashCode();
            hashCode = hashCode * -1521134295 + InEncodeLevel.GetHashCode();
            return hashCode;
        }

        public void Deconstruct(out int xOffset, out int yOffset, out int zLimit, out bool wantsEncode, out int inEncodeLevel)
        {
            xOffset = this.XOffset;
            yOffset = this.YOffset;
            zLimit = this.ZLimit;
            wantsEncode = this.WantsEncode;
            inEncodeLevel = this.InEncodeLevel;
        }

        public static implicit operator (int xOffset, int yOffset, int zLimit, bool WantsEncode, int inEncodeLevel)(ProgramStep value)
        {
            return (value.XOffset, value.YOffset, value.ZLimit, value.WantsEncode, value.InEncodeLevel);
        }

        public static implicit operator ProgramStep((int xOffset, int yOffset, int zLimit, int inEncodeLevel) value)
        {
            return new ProgramStep(value.xOffset, value.yOffset, value.zLimit, value.inEncodeLevel);
        }
    }
}
