using System;
using System.IO;
using System.Collections.Generic;

public class Class1
{
	public Class1()
	{
        int GetMassFuel(string file)
        {
            FileStream input = File.OpenRead(file);
            byte[] content = new byte[input.Length];
            if (input.Length != input.Read(content, 0, (int)input.Length))
                throw new Exception("nicht alles gelesen");
            input.Close();
            char[] text = new char[content.Length];
            content.CopyTo(text, 0);
            string[] values = string.Join("", text).Split('\n');
            int fuel = 0;
            foreach(string value in values)
            {
                if (!int.TryParse(value.Trim('#'), out int mass))
                    throw new Exception("Ungültiger Wert: '"+value+"'");
                fuel += mass / 3 - 2;
            }
            return fuel;
        }

        int GetFuelFuel(int fuel)
        {
            if (fuel <= 0) return 0;
            return fuel + GetFuelFuel(fuel / 3 - 2);
        }
    }
}
