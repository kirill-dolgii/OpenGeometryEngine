using netDxf;
using OpenGeometryEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Translators.Utils;

namespace Translators;

public class DxfTranslator
{
	public static bool Read(string path, out ICollection<IBoundedCurve> output)
	{
		var fileInfo = new FileInfo(path);
		output = Array.Empty<IBoundedCurve>();
		
		if (!fileInfo.Exists)
		{
			return false;
		}

		var doc = DxfDocument.Load(path);

		if (doc == null)
		{
			return false;
		}

		var ogeArcs = doc.Arcs.Select(Converters.ToArc).ToList();
		var ogeLines = doc.Lines.Select(Converters.ToLineSegment).ToList();

		output = ogeArcs.Cast<IBoundedCurve>().Concat(ogeLines).ToList();
		return output.Any();
	}

	public static bool Write(string path, ICollection<IBoundedCurve> curves)
	{
		var fileInfo = new FileInfo(path);
		if (fileInfo.Exists)
		{
			return false;
		}

		var doc = new DxfDocument(netDxf.Header.DxfVersion.AutoCad2018);

		foreach (var curve in curves )
		{
			doc.AddEntity(BackWardsConverters.ToDxfEntity(curve));
		}

		return doc.Save(path);
	}
}
