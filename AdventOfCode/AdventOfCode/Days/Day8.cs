using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdventOfCode.Tools;

namespace AdventOfCode.Days
{
    class Day8 : IDay
    {
        //Layer, Y, X
        private int[][][] image;
        private int width;
        private int height;
        private readonly char[] pixels = { ' ', '█', '░' };

        public string Solve(string input, bool part2)
        {
            //input = "123456789012";
            Console.WriteLine("Image Analizer v1.0");
            Console.WriteLine("Data recieved");
            width = ConsoleAssist.GetUserInput("Please enter the width of the image");
            height = ConsoleAssist.GetUserInput("Please enter the height of the image");
            Console.WriteLine("Constructing image...");
            CreateImage(LoadImageData(input));
            if (part2) return Render();
            int leastNulAmount = -1;
            int leastNulLayer = -1;
            Console.WriteLine("Analyzing image...");
            for (int i = 0; i < image.Length; i++)
            {
                int[] statistics = GetLayerStatistics(image[i]);
                if(statistics[0] < leastNulAmount || leastNulAmount < 0)
                {
                    leastNulAmount = statistics[0];
                    leastNulLayer = i;
                }
            }
            int[] layerStatistic = GetLayerStatistics(image[leastNulLayer]);
            return (layerStatistic[1] * layerStatistic[2]).ToString();
        }

        private int[] LoadImageData(string dataString)
        {
            MatchCollection digits = Regex.Matches(dataString, "\\d");
            List<int> data = new List<int>();
            foreach(Match digit in digits)
            {
                data.Add(int.Parse(digit.Value));
            }
            return data.ToArray();
        }

        private void CreateImage(int[] dataStream)
        {
            int position = 0;
            List<int[][]> Layers = new List<int[][]>();
            while(position < dataStream.Length)
            {
                List<int[]> Rows = new List<int[]>();
                for(int y = 0; y < height; y++)
                {
                    List<int> values = new List<int>();
                    for(int x = 0; x < width; x++)
                    {
                        values.Add(dataStream[position++]);
                    }
                    Rows.Add(values.ToArray());
                }
                Layers.Add(Rows.ToArray());
            }
            image = Layers.ToArray();
        }

        private int[] GetLayerStatistics(int[][] imageLayer)
        {
            int[] result = new int[10];

            for(int y = 0; y < imageLayer.Length; y++)
            {
                for (int x = 0; x < imageLayer[y].Length; x++)
                {
                    result[imageLayer[y][x]]++;
                }
            }

            return result;
        }

        private string Render()
        {
            StringBuilder RenderedImage = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelValue = 2;
                    foreach(int[][] layer in image)
                    {
                        pixelValue = layer[y][x];
                        if (pixelValue != 2) break;
                    }
                    RenderedImage.Append(pixels[pixelValue]);
                }
                RenderedImage.AppendLine();
            }
            return RenderedImage.ToString();
        }
    }
}
