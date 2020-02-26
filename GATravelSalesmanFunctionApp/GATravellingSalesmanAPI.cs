using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using GATravellingSalesman;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SixLabors.ImageSharp.Formats.Jpeg;
using Travelling_Salesman;

namespace GATravelSalesmanFunctionApp
{
    public static class GATravellingSalesmanAPI
    {
        const int ActualHeight = 1500;
        const int ActualWidth = 1500;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("FindOptimisedPath")]
        public static async Task<IActionResult> FindOptimisedPath(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "OptimisedPath")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Start a new optimisation job.");

           // string NumCities = req.Query["NumCities"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<GATravellingSalesmanInput>(requestBody);

            //=========run optimisation job
            var GA = new GATravellingSalesmanContoller();
            CreateCities(GA, input);
            var TraPathChosen = GA.SolveTrad();
            var GAPathChosen = GA.SolveGA();
            //\===========
            var img = Draw();
            DrawCities(img, GA);
            DrawPath(img, GA, GAPathChosen, true);
            DrawPath(img, GA,TraPathChosen,false);
            var imgAsBase64 = "";
            using (var outputStream = new MemoryStream())
            {
                img.Save(outputStream, new JpegEncoder());
                var bytes = outputStream.ToArray();
                imgAsBase64 = Convert.ToBase64String(bytes);
            }

            var output = new GATravellingSalesmanOutput()
            {
                BestLength= GAPathChosen.Length,
                NextNeighbourLength= TraPathChosen.Length,
                Image = imgAsBase64
            };
            return new OkObjectResult(output);
        }

      
        private static void CreateCities(GATravellingSalesmanContoller GA, GATravellingSalesmanInput input)
        {
            GA.NumCities = input.NumCities;
            GA.PopulationSize = input.PopulationSize;
            GA.CrossoverPercentage = input.CrossoverPercentage;
            GA.MutationPercentage = input.MutationPercentage;
            GA.NumIterations = input.NumIterations;

            GA.ActualHeight = ActualHeight;
            GA.ActualWidth = ActualWidth;

            GA.CreateRandom();
        }

        private static Image Draw()
        {
            Image img = new Image<Rgba32>(ActualWidth, ActualHeight);

            //PathBuilder pathBuilder = new PathBuilder();
            //pathBuilder.SetOrigin(new PointF(500, 0));
            //pathBuilder.AddBezier(new PointF(50, 450), new PointF(200, 50), new PointF(300, 50), new PointF(450, 450));
            //// add more complex paths and shapes here.

            //IPath path = pathBuilder.Build();

            //// For production application we would recomend you create a FontCollection
            //// singleton and manually install the ttf fonts yourself as using SystemFonts
            //// can be expensive and you risk font existing or not existing on a deployment
            //// by deployment basis.
            //var font = SystemFonts.CreateFont("Arial", 39, FontStyle.Regular);

            //string text = "Hello World Hello World Hello World Hello World Hello World";
            //var textGraphicsOptions = new TextGraphicsOptions(true) // draw the text along the path wrapping at the end of the line
            //{
            //    WrapTextWidth = path.Length
            //};

            //// lets generate the text as a set of vectors drawn along the path


            //var glyphs = TextBuilder.GenerateGlyphs(text, path, new RendererOptions(font, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
            //{
            //    HorizontalAlignment = textGraphicsOptions.HorizontalAlignment,
            //    TabWidth = textGraphicsOptions.TabWidth,
            //    VerticalAlignment = textGraphicsOptions.VerticalAlignment,
            //    WrappingWidth = textGraphicsOptions.WrapTextWidth,
            //    ApplyKerning = textGraphicsOptions.ApplyKerning
            //});

            //img.Mutate(ctx => ctx
            //    .Fill(Color.White) // white background image
            //    .Draw(Color.Gray, 3, path) // draw the path so we can see what the text is supposed to be following
            //    .Fill((GraphicsOptions)textGraphicsOptions, Color.Black, glyphs));

            ////img.Save("output/wordart.png");

            return img;
        }


        private static void DrawCities(Image canvas, GATravellingSalesmanContoller GA)
        {
            canvas.Mutate(ctx => ctx
              .Fill(Color.White) // white background image
              );
            for (int i = 0; i < GA.Cities.Count; i++)
            {
                var city = GA.Cities[i];
                var displayElement = new EllipsePolygon(city.X, city.Y, City.ClickRadius * 2, City.ClickRadius * 2);

                canvas.Mutate(ctx => ctx
                .Fill(i == 0 ? Color.Red : Color.Green, displayElement));

            }
        }

        private static void DrawPath(Image canvas, GATravellingSalesmanContoller GA, PathChosen path, bool optimised)
        {
            City firstCity = GA.Cities[0];

            PathBuilder pathBuilder = new PathBuilder();
       
             var points = new List<PointF>();
            points.Add(new PointF(firstCity.X, firstCity.Y));
            for (int i = 0; i < path.CityIndexes.Count; i++)
            {
                City city = GA.Cities[path.CityIndexes[i]];
                var p = new PointF(city.X, city.Y);
                points.Add(p);

            }

            pathBuilder.AddLines(points);
            pathBuilder.CloseFigure();
            IPath spath = pathBuilder.Build();
            canvas.Mutate(ctx => ctx
             .Draw(optimised? Color.Blue:Color.Gray, 3, spath));

          
        }

    }
}
