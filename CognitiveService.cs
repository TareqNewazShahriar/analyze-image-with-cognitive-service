using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace image_procssing_for_auto_cropping_yacht_images;

public class CognitiveService
{	
	/*
	* AUTHENTICATE
	* Creates a Computer Vision client used by each example.
	*/
	public ComputerVisionClient Authenticate(string endpoint, string key)
	{
		ComputerVisionClient client = 
			new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
			{
				Endpoint = endpoint
			};

		return client;
	}

	/* 
	* ANALYZE IMAGE - URL IMAGE
	* Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult/gory content,
	* brands, celebrities, landmarks, color scheme, and image types.
	*/
	public async Task<ImageAnalysis> GetCognitiveServiceData(ComputerVisionClient client, string imageUrl)
	{
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
		
		// Analyze the URL image 
		ImageAnalysis imageAnalysis = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);

		return imageAnalysis;
	}

	public static void PrintServiceData(ImageAnalysis imageAnalysis)
	{	
		// Sunmarizes the image content.
		Console.WriteLine("Summary:");
		foreach (var caption in imageAnalysis.Description.Captions)
		{
			Console.WriteLine($"\"{caption.Text}\" with confidence {caption.Confidence}");
		}
		Console.WriteLine();

		// Display categories the image is divided into.
		Console.WriteLine("Categories:");
		foreach (var category in imageAnalysis.Categories)
		{
			Console.WriteLine($"\"{category.Name}\" (confidence: {category.Score})");
		}
		Console.WriteLine();

		// Image tags and their confidence score
		Console.WriteLine("Tags:");
		foreach (var tag in imageAnalysis.Tags)
		{
			Console.WriteLine($"\"{tag.Name}\" (confidence: {tag.Confidence})");
		}
		Console.WriteLine();

		// Objects
		Console.WriteLine("Objects:");
		foreach (var obj in imageAnalysis.Objects)
		{
			Console.WriteLine($"\"{obj.ObjectProperty}\" with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
				$"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
		}
		Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(imageAnalysis.Objects));
		Console.WriteLine();

		// Adult or racy content, if any.
		Console.WriteLine("Adult:");
		Console.WriteLine($"Has adult content: \"{imageAnalysis.Adult.IsAdultContent}\" with confidence {imageAnalysis.Adult.AdultScore}");
		Console.WriteLine($"Has racy content: \"{imageAnalysis.Adult.IsRacyContent}\" with confidence {imageAnalysis.Adult.RacyScore}");
		Console.WriteLine($"Has gory content: \"{imageAnalysis.Adult.IsGoryContent}\" with confidence {imageAnalysis.Adult.GoreScore}");
		Console.WriteLine();

		// Identifies the color scheme.
		Console.WriteLine("Color Scheme:");
		Console.WriteLine("Is black and white?: " + imageAnalysis.Color.IsBWImg);
		Console.WriteLine("Accent color: " + imageAnalysis.Color.AccentColor);
		Console.WriteLine("Dominant background color: " + imageAnalysis.Color.DominantColorBackground);
		Console.WriteLine("Dominant foreground color: " + imageAnalysis.Color.DominantColorForeground);
		Console.WriteLine("Dominant colors: " + string.Join(",", imageAnalysis.Color.DominantColors));
		Console.WriteLine();

		// Detects the image types.
		Console.WriteLine("Image Type:");
		Console.WriteLine("Clip Art Type: " + imageAnalysis.ImageType.ClipArtType);
		Console.WriteLine("Line Drawing Type: " + imageAnalysis.ImageType.LineDrawingType);
		Console.WriteLine();
	}
}
