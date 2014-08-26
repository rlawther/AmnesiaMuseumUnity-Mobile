using UnityEngine;
using System.Collections.Generic;
using ReadWriteCsv;
using System;

public class AutographerParser : MetadataParser {
	public class AutographerMetaDataItem : MetaDataItem {
		public string id;
		public string dt;
		public float AccuracyX;
		public float AccuracyY;
		public float AccuracyZ;
		public float MagnitudeX;
		public float MagnitudeY;
		public float MagnitudeZ;
		public float Red;
		public float Green;
		public float Blue;
		public float Luminance;
		public float Temperature;
		public float Gs;
		public float HorizontalError;
		public float VerticalError;
	}

	protected TextAsset lastFileParsed;
	
	protected override string encodingType {
		get {
			return "Autographer";
		}
	}
	public enum ImageResolution
	{
		Low,
		Medium,
		High
	};


	public ImageResolution imageResolution = ImageResolution.Low;
	
	private string ImageResolutionPrefix(ImageResolution ir) {
		if (ir == ImageResolution.High) {
			return "";
		} else if (ir == ImageResolution.Medium) {
			return "640_480/";
		} else {
			return "256_192/";
		}
	}
	
	private string ImageResolutionSuffix(ImageResolution ir) {
		if (ir == ImageResolution.High) {
			return "e";
		} else if (ir == ImageResolution.Medium) {
			return "4";
		} else {
			return "2";
		}
	}
	
	private string HourFolder(string baseFileName) {
		return baseFileName.Substring(baseFileName.Length - 6, 2) + "/";
	}
	
	public bool allowInterp = true;
	sealed override protected List<MetaDataItem> doParseFile(TextAsset fileToParse) {
		this.lastFileParsed = fileToParse;
		List<MetaDataItem> result = new List<MetaDataItem>();
		//first open the file.
		string [,] csvData = CSVReader.SplitCsvGrid(fileToParse.text);
		Debug.Log (csvData[0, 0]);
		List<string> columns = new List<string>();
		int rowIndex, colIndex;
		int size = csvData.GetLength(1);
		/* Not sure why i have to subtract 2. Something to do with the CSVReader I guess? */
		for (rowIndex = 0; rowIndex < csvData.GetLength(1) - 2; rowIndex++)
		{
			if (rowIndex == 0)
			{
				for (colIndex = 0; colIndex < csvData.GetLength(0); colIndex++)
				{
					columns.Add(csvData[colIndex, rowIndex]);
				}
			}
			else
			{
				var dataItem = new AutographerMetaDataItem();
				dataItem.id = csvData[columns.IndexOf("id"), rowIndex];
				dataItem.dt = csvData[columns.IndexOf("dt"), rowIndex];
				dataItem.latitude = float.Parse(csvData[columns.IndexOf("lat_smooth"), rowIndex]);
				dataItem.longitude = float.Parse(csvData[columns.IndexOf("lon_smooth"), rowIndex]);
				dataItem.filename = csvData[columns.IndexOf("imgFile"), rowIndex];
				dataItem.heading = float.Parse(csvData[columns.IndexOf("heading"), rowIndex]);
				dataItem.priority = int.Parse(csvData[columns.IndexOf("priority"), rowIndex]);
				//dataItem.stream = int.Parse(csvData[columns.IndexOf("stream"), rowIndex]);
				result.Add (dataItem);
				//Debug.Log ("added row " + rowIndex + " of " + size);
			}
		}
		
		int lastValueIndex = 0;		
		//now go through again, finding the last index with non-zero longitude
		//first item is guaranteed to be non-zero.
		//a "value" index is an index with a non zero value.
		this.min = new MetaDataItem();
		this.max = new MetaDataItem();
		
		for (int i = 0; i < result.Count; i++) {

			if (i == 0) {
				this.min.longitude = result[i].longitude;

				this.min.latitude = result[i].latitude;
				this.min.altitude = result[i].altitude;

				this.max.longitude = result[i].longitude;
				this.max.latitude = result[i].latitude;
				this.max.altitude = result[i].altitude;
			}
			if (result[i].longitude != 0.0) {
				//interpolate between previous value index and this value index.
				
				int startTerpIndex = lastValueIndex;
				int endTerpIndex = i;
				
				int numItems = i - (startTerpIndex + 1);
				
				for (int j = startTerpIndex + 1; j < i; j++ ) {
					
					int numerator = j - startTerpIndex; // starts from 1.
					int denominator = numItems + 1;
					float terpAmount = numerator/(float)denominator;
					result[j].latitude = Mathf.Lerp(result[startTerpIndex].latitude,result[endTerpIndex].latitude,terpAmount);
					result[j].longitude = Mathf.Lerp(result[startTerpIndex].longitude,result[endTerpIndex].longitude,terpAmount);
					result[j].altitude = Mathf.Lerp(result[startTerpIndex].altitude,result[endTerpIndex].altitude,terpAmount);					
				}
				lastValueIndex = i;
				
				this.min.latitude = result[i].latitude < this.min.latitude ? result[i].latitude : this.min.latitude;
				this.min.longitude = result[i].longitude < this.min.longitude ? result[i].longitude : this.min.longitude;
				this.min.altitude = result[i].altitude < this.min.altitude ? result[i].altitude : this.min.altitude;
				
				this.max.latitude = result[i].latitude > this.max.latitude ? result[i].latitude : this.max.latitude;
				this.max.longitude = result[i].longitude > this.max.longitude ? result[i].longitude : this.max.longitude;
				this.max.altitude = result[i].altitude > this.max.altitude ? result[i].altitude : this.max.altitude;
								
			} 

		}
		//now cull everything past the last index.
		while (result.Count > lastValueIndex+1) {
			result.RemoveAt (result.Count - 1);
		}

		return result;
		
	}
}
