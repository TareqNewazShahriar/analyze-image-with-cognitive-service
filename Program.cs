using System.Text.Json.Nodes;
using System.Drawing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Net;
using System.Drawing.Imaging;

namespace ComputerVisionQuickStart
{
	public class Program
	{
		private const string OUTPUT_DIRECTORY = "output";
		private const string ANALYZE_URL_IMAGE = "https://cdn.bluebnc.com/images/boat/1554/4cf464c4-7b8b-4742-9d77-8c2b7b2262f3-large.jpg";

		public static async Task Main()
		{
			JsonNode? resourceValues = ReadAppSettings();

			string endpoint = resourceValues?["azure-resource-endpoint-url"]?.ToString() ?? "";
			string key = resourceValues?["subscription-key1"]?.ToString() ?? "";

			// Create a client
			ComputerVisionClient client = Authenticate(endpoint, key);

			// Analyze an image to get features and other properties.
			var AnalyzedResult = await AnalyzeImageUrl(client, ANALYZE_URL_IMAGE);

			DrawOnImage(ANALYZE_URL_IMAGE, AnalyzedResult);
		}

		/*
		* AUTHENTICATE
		* Creates a Computer Vision client used by each example.
		*/
		public static ComputerVisionClient Authenticate(string endpoint, string key)
		{
			ComputerVisionClient client =
				new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
				{ Endpoint = endpoint };
			return client;
		}

		/* 
		* ANALYZE IMAGE - URL IMAGE
		* Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult/gory content,
		* brands, celebrities, landmarks, color scheme, and image types.
		*/
		public static async Task<ImageAnalysis> AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
		{
			Console.WriteLine("----------------------------------------------------------");
			Console.WriteLine("ANALYZE IMAGE - URL");
			Console.WriteLine();

			// Creating a list that defines the features to be extracted from the image. 
			var features = new List<VisualFeatureTypes?>()
			{
				VisualFeatureTypes.Categories,
				VisualFeatureTypes.Description,
				VisualFeatureTypes.Faces,
				VisualFeatureTypes.ImageType,
				VisualFeatureTypes.Tags,
				VisualFeatureTypes.Adult,
				VisualFeatureTypes.Color,
				VisualFeatureTypes.Brands,
				VisualFeatureTypes.Objects
			};

			Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
			Console.WriteLine();
			// Analyze the URL image 
			ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);
			
			// Sunmarizes the image content.
			Console.WriteLine("Summary:");
			foreach (var caption in results.Description.Captions)
			{
				Console.WriteLine($"\"{caption.Text}\" with confidence {caption.Confidence}");
			}
			Console.WriteLine();

			// Display categories the image is divided into.
			Console.WriteLine("Categories:");
			foreach (var category in results.Categories)
			{
				Console.WriteLine($"\"{category.Name}\" (confidence: {category.Score})");
			}
			Console.WriteLine();

			// Image tags and their confidence score
			Console.WriteLine("Tags:");
			foreach (var tag in results.Tags)
			{
				Console.WriteLine($"\"{tag.Name}\" (confidence: {tag.Confidence})");
			}
			Console.WriteLine();

			// Objects
			Console.WriteLine("Objects:");
			foreach (var obj in results.Objects)
			{
				Console.WriteLine($"\"{obj.ObjectProperty}\" with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
					$"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
			}
			Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(results.Objects));
			Console.WriteLine();

			// Adult or racy content, if any.
			Console.WriteLine("Adult:");
			Console.WriteLine($"Has adult content: \"{results.Adult.IsAdultContent}\" with confidence {results.Adult.AdultScore}");
			Console.WriteLine($"Has racy content: \"{results.Adult.IsRacyContent}\" with confidence {results.Adult.RacyScore}");
			Console.WriteLine($"Has gory content: \"{results.Adult.IsGoryContent}\" with confidence {results.Adult.GoreScore}");
			Console.WriteLine();

			// Identifies the color scheme.
			Console.WriteLine("Color Scheme:");
			Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
			Console.WriteLine("Accent color: " + results.Color.AccentColor);
			Console.WriteLine("Dominant background color: " + results.Color.DominantColorBackground);
			Console.WriteLine("Dominant foreground color: " + results.Color.DominantColorForeground);
			Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
			Console.WriteLine();

			// Detects the image types.
			Console.WriteLine("Image Type:");
			Console.WriteLine("Clip Art Type: " + results.ImageType.ClipArtType);
			Console.WriteLine("Line Drawing Type: " + results.ImageType.LineDrawingType);
			Console.WriteLine();

			return results;
		}

		public static void DrawOnImage(string imageUrl, ImageAnalysis analyzedResult)
		{
			Image image = SaveImage(imageUrl, @$"{OUTPUT_DIRECTORY}\unprocessed-image.jpg", ImageFormat.Jpeg);

			using(Font arialFont =  new Font("Arial", 30, FontStyle.Bold))
			using (Graphics g = Graphics.FromImage(image))
			{
				var fillColor = Color.FromArgb(50, Color.Black);
				var shadowBrush = new System.Drawing.SolidBrush(fillColor);
				foreach (var obj in analyzedResult.Objects)
				{
					PointF objectLocation = new PointF(obj.Rectangle.X, obj.Rectangle.Y);
					var rect = new RectangleF(objectLocation, new SizeF(obj.Rectangle.W, obj.Rectangle.H));
					g.DrawRectangle(new Pen(Color.FromArgb(200, Color.Black), 2), rect.X, rect.Y, rect.Width, rect.Height);
					g.FillRectangles( shadowBrush, new RectangleF[]{ rect });
					
					g.DrawString($"{obj.ObjectProperty} (confidence: {obj.Confidence})", arialFont, Brushes.WhiteSmoke, objectLocation);
				}	

				g.DrawString($"{analyzedResult.Description.Captions.First().Text} (confidence: {analyzedResult.Description.Captions.First().Confidence})", arialFont, Brushes.WhiteSmoke, new PointF(10, 10));
			}

			image.Save(@$"{OUTPUT_DIRECTORY}\{analyzedResult.Description.Captions.First().Text}.jpg", ImageFormat.Jpeg);
		}

		public static Image SaveImage(string imageUrl, string filename, ImageFormat format)
		{    
			WebClient client = new WebClient();
			Stream stream = client.OpenRead(imageUrl);
			Bitmap bitmap;  bitmap = new Bitmap(stream);

			if (bitmap != null)
			{
				bitmap.Save(filename, format);
			}
				
			stream.Flush();
			stream.Close();
			client.Dispose();

			return Image.FromFile(filename);
		}

		public static JsonNode? ReadAppSettings()
		{
			string jsonString = System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\appSettings.json");

			JsonNode? values = JsonNode.Parse(jsonString);

			return values;
		}
	}
}
