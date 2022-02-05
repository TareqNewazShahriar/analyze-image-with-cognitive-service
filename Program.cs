using System.Text.Json.Nodes;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;


// See https://aka.ms/new-console-template for more information

namespace ComputerVisionQuickStart
{
	public class Program
	{
		private const string subscriptionKey = "";
		private const string endpoint = "";

		private const string ANALYZE_URL_IMAGE = "https://cdn-dev.bluebnc.com/images/boat/1555/9e3ecd3d-d3d3-4eb7-9af6-cee33157410e-large.jpg";

		public static void Main()
		{
			JsonNode? resourceValues = ReadJson();

			string key = resourceValues?["subscription-key1"]?.ToString() ?? "";
			string endpoint = resourceValues?["azure-resource-endpoint-url"]?.ToString() ?? "";

			// Create a client
			ComputerVisionClient client = Authenticate(endpoint, key);

			// Analyze an image to get features and other properties.
			AnalyzeImageUrl(client, ANALYZE_URL_IMAGE).Wait();
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
		public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
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
				Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
			}
			Console.WriteLine();

			// Display categories the image is divided into.
			Console.WriteLine("Categories:");
			foreach (var category in results.Categories)
			{
				Console.WriteLine($"{category.Name} with confidence {category.Score}");
			}
			Console.WriteLine();

			// Image tags and their confidence score
			Console.WriteLine("Tags:");
			foreach (var tag in results.Tags)
			{
				Console.WriteLine($"{tag.Name} {tag.Confidence}");
			}
			Console.WriteLine();

			// Objects
			Console.WriteLine("Objects:");
			foreach (var obj in results.Objects)
			{
				Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
					$"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
			}
			Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(results.Objects));
			Console.WriteLine();

			// Adult or racy content, if any.
			Console.WriteLine("Adult:");
			Console.WriteLine($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
			Console.WriteLine($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");
			Console.WriteLine($"Has gory content: {results.Adult.IsGoryContent} with confidence {results.Adult.GoreScore}");
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
		}

		public static JsonNode? ReadJson()
		{
			string jsonString = System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\secrets.json");

			JsonNode? values = JsonNode.Parse(jsonString);

			return values;
		}
	}
}
