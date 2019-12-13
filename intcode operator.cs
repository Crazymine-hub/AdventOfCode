using System;

public class Class1
{
    int[] ProcessCode(int[] program)
    {
        for (int i = 0; i < program.Length; i+=4)
        {
            switch (program[i])
            {
                case 1: program[program[i + 3]] = program[program[i + 1]] + program[program[i + 2]];
                    break;
                case 2: program[program[i + 3]] = program[program[i + 1]] * program[program[i + 2]];
                    break;
                case 99:
                    return program;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("OPCode {1}", program[i]));
            }
        }
        return program;
    }
}
