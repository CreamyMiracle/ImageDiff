using Fclp;
using Fclp.Internals;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageDiff
{
    public class Program
    {
        private static string? _originalImagePath = @"C:\Users\Rajala\Desktop\original.png";
        private static string? _referenceImagePath = @"C:\Users\Rajala\Desktop\reference.png";
        private static bool _parsingOk = true;
        private static string[] _debugArgs = new string[] { "-o", _originalImagePath, "-f", _referenceImagePath };

        [STAThread]
        static void Main(string[] args)
        {
            if (Parse(args.Count() == 0 ? _debugArgs : args))
            {
                CompareImages();
            }

            Thread.CurrentThread.Join();
        }

        private static bool Parse(string[] args)
        {
            var p = new FluentCommandLineParser();

            p.Setup<string>('o', "original")
             .Callback(value => _originalImagePath = value)
             .Required()
             .WithDescription("Path to original image");

            p.Setup<string>('f', "reference")
             .Callback(value => _referenceImagePath = value)
             .Required()
             .WithDescription("Path to reference image");

            p.SetupHelp("?", "help")
             .Callback(text =>
             {
                 _parsingOk = false;
                 Console.WriteLine(text);
             });

            var parseResult = p.Parse(args);
            if (parseResult.HasErrors)
            {
                p.HelpOption.ShowHelp(p.Options);
                _parsingOk = false;
            }
            return _parsingOk;
        }

        private static void CompareImages()
        {
            FileInfo originalFile = new FileInfo(_originalImagePath);
            FileInfo referenceFile = new FileInfo(_referenceImagePath);

            if (originalFile.Exists && referenceFile.Exists)
            {
                Bitmap originalBmp = new Bitmap(_originalImagePath);
                Bitmap referenceBmp = new Bitmap(_referenceImagePath);

                if (originalBmp.Size == referenceBmp.Size)
                {
                    double similarity = Similarity(originalBmp, referenceBmp);
                    Console.WriteLine(similarity);
                }
            }

        }

        public static double Similarity(Bitmap bmp1, Bitmap bmp2)
        {
            List<KeyValuePair<int, int>> differentPixels = new List<KeyValuePair<int, int>>();

            for (int x = 0; x < bmp1.Width; x++)
            {
                for (int y = 0; y < bmp1.Height; y++)
                {
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                    {
                        differentPixels.Add(new KeyValuePair<int, int>(x, y));
                    }
                }
            }

            double totalPixelCount = bmp1.Width * bmp1.Height;
            double similarPixelCount = totalPixelCount - differentPixels.Count;

            return similarPixelCount / totalPixelCount;
        }
    }
}