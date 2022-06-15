using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

public class ProcessImage
{
	private string OUTPUT_DIRECTORY => "output";

	public Image MarkObjectsOnImage(string imagePath, ImageAnalysis analyzedResult)
	{
		Image image = SaveImage(imagePath, @$"{OUTPUT_DIRECTORY}\unprocessed-image.jpg", ImageFormat.Jpeg);

		foreach (var obj in analyzedResult.Objects)
		{
			WriteTextOnImage($"{obj.ObjectProperty} (confidence: {obj.Confidence})", obj.Rectangle.X, obj.Rectangle.Y, ref image);
		}

		WriteTextOnImage($"{analyzedResult.Description.Captions.First().Text} (confidence: {analyzedResult.Description.Captions.First().Confidence})", 10, 10, ref image);
		
		image.Save(@$"{OUTPUT_DIRECTORY}\{analyzedResult.Description.Captions.First().Text}.jpg", ImageFormat.Jpeg);

		return image;
	}

	public void DrawOnImage(int x, int y, int width, int height, ref Image image)
	{
		using (Graphics g = Graphics.FromImage(image))
		{
			var fillColor = Color.FromArgb(50, Color.Black);
			var shadowBrush = new System.Drawing.SolidBrush(fillColor);

			PointF objectLocation = new PointF(x, y);
			var rect = new RectangleF(objectLocation, new SizeF(width, height));
			g.DrawRectangle(new Pen(Color.FromArgb(200, Color.Black), 2), rect.X, rect.Y, rect.Width, rect.Height);
			g.FillRectangles(shadowBrush, new RectangleF[]{ rect });
		}
	}

	public void WriteTextOnImage(string text, int x, int y, ref Image image)
	{
		using(Font arialFont =  new Font("Arial", 30, FontStyle.Bold))
		using (Graphics g = Graphics.FromImage(image))
		{
			var point = new PointF(x, y);
			g.DrawString(text, arialFont, Brushes.WhiteSmoke, point);
		}
	}

	public Image SaveImage(string imageUrl, string filename, ImageFormat format)
	{    
		WebClient client = new WebClient();
		Stream stream = client.OpenRead(imageUrl);
		Bitmap bitmap;  bitmap = new Bitmap(stream);

		if (bitmap != null)
		{
			bitmap.Save(filename, format);
		}
		
		return Image.FromFile(filename);
	}

	public bool IsSingleBoatImage(ImageAnalysis imageAnalysis, Image image)
	{
		var objectListOrdered = imageAnalysis.Objects.OrderByDescending(x => x.Rectangle.W * x.Rectangle.H);
		var largestObject = objectListOrdered.FirstOrDefault();

		if(largestObject is null)
		{
			Console.WriteLine("No object detected in the image!");
			return false;
		}

		var largestObejct2nd = objectListOrdered.Skip(1).FirstOrDefault();
		
		int percentage1st = (int)((largestObject.Rectangle.W * largestObject.Rectangle.H * 100) / (image.Height * image.Width));

		int percentage2nd = largestObejct2nd is object ?
			(int)((largestObejct2nd.Rectangle.W * largestObejct2nd.Rectangle.H * 100) / (image.Height * image.Width))
			: 0;

		bool isBiggestObjectABoat = false;
		var obj = new ObjectHierarchy(largestObject.ObjectProperty, largestObject.Confidence, largestObject.Parent);
		while(obj is object)
		{
			if(obj.ObjectProperty.ToLower().Contains("boat") 
				|| obj.ObjectProperty.ToLower().Contains("watercraft"))
			{
				isBiggestObjectABoat = true;
				break;
			}
			else
			{
				obj = obj.Parent;
			}
		}

		bool isABoatImage = isBiggestObjectABoat && percentage1st > 30;

		Console.WriteLine($"Largest object: {largestObject.ObjectProperty ?? "(null)"} which takes {percentage1st}% of the image.");
		Console.WriteLine($"2nd largest object: {largestObejct2nd?.ObjectProperty ?? "(null)"} which takes {percentage2nd}% of the image.");
		Console.WriteLine($"Is this an image of a boat? {isABoatImage}");

		return isABoatImage;
	}

	public bool DrawCropArea(ImageAnalysis imageAnalysis, Image image)
	{
		return false;
	}
}
