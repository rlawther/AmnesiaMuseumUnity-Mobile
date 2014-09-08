using UnityEngine;
using System.Collections;
using System.IO;

namespace Toolbelt {
public class BinkQualitySelector {
	private static QualityLevel qualityLevel = QualityLevel.Normal;
	public enum QualityLevel {
		Low,
		HalfRate,
		Normal
	}
	
	public static void setQuality(QualityLevel ql) {
		BinkQualitySelector.qualityLevel = ql;
	}
	//given a url, return the equivalent URL based on the current qualityLevel, or the optionally supplied qualitylevel.
	public static string getUrl(string url, QualityLevel? ql = null) {
		QualityLevel chosenLevel = BinkQualitySelector.qualityLevel;
		if (ql.HasValue) {
			chosenLevel = ql.Value;
		}
		
		if (chosenLevel == QualityLevel.Low) {	
			string url2 = Path.GetDirectoryName(url) + "/[LowRes]" + Path.GetFileName(url);
		
			if (File.Exists(url2)) {
				return url2;
			} else {
				chosenLevel = QualityLevel.HalfRate; //cycle to next possible quality level.
			}
		}
		
		if (chosenLevel == QualityLevel.HalfRate) {
			string url2 = Path.GetDirectoryName(url) + "/[HalfRate]" + Path.GetFileName(url);
		
			if (File.Exists(url2)) {
				return url2;
			}
		}
		
		return url;
	}
	
}
}