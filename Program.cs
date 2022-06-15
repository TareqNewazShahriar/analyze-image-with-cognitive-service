using System.Text.Json.Nodes;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace image_procssing_for_auto_cropping_yacht_images;

public class Program
{
	private const string ANALYZE_URL_IMAGE = "https://www.burgessyachts.com/sitefiles/burgess/medialibrary/landing%20page%20images/sell/lrtitania_00005137_vb1081408.jpg?ext=.jpg&height=0&quality=&mode=";

	public static async Task Main()
	{
		try
		{
			JsonNode? resourceValues = ReadAppSettings();
			string endpoint = resourceValues?["azure-resource-endpoint-url"]?.ToString() ?? "";
			string key = resourceValues?["subscription-key1"]?.ToString() ?? "";

			var cognitiveService = new CognitiveService();
			// Create a client
			ComputerVisionClient client = cognitiveService.Authenticate(endpoint, key);
			// Analyze an image to get features and other properties.
			var analyzedResult = await cognitiveService.GetCognitiveServiceData(client, ANALYZE_URL_IMAGE);

			var ProcessImage = new ProcessImage();
			var modifiedImage = ProcessImage.MarkObjectsOnImage(ANALYZE_URL_IMAGE, analyzedResult);
			ProcessImage.IsSingleBoatImage(analyzedResult, modifiedImage);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error occurred! [Error: {ex.Message}]");
		}
	}

	public static JsonNode? ReadAppSettings()
	{
		string jsonString = System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\appSettings.json");

		JsonNode? values = JsonNode.Parse(jsonString);

		return values;
	}
}
