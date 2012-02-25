﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.Documentation
{
	/// <summary>
	/// Provides documentation from an .xml file (as generated by the Microsoft C# compiler).
	/// </summary>
	/// <remarks>
	/// This class first creates an in-memory index of the .xml file, and then uses that to read only the requested members.
	/// This way, we avoid keeping all the documentation in memory.
	/// </remarks>
	[Serializable]
	public class XmlDocumentationProvider : IDocumentationProvider, IDeserializationCallback
	{
		#region Cache
		sealed class XmlDocumentationCache
		{
			readonly KeyValuePair<string, string>[] entries;
			int pos;
			
			public XmlDocumentationCache(int size = 50)
			{
				if (size <= 0)
					throw new ArgumentOutOfRangeException("size", size, "Value must be positive");
				this.entries = new KeyValuePair<string, string>[size];
			}
			
			internal string Get(string key)
			{
				foreach (var pair in entries) {
					if (pair.Key == key)
						return pair.Value;
				}
				return null;
			}
			
			internal void Add(string key, string value)
			{
				entries[pos++] = new KeyValuePair<string, string>(key, value);
				if (pos == entries.Length)
					pos = 0;
			}
		}
		#endregion
		
		[Serializable]
		struct IndexEntry : IComparable<IndexEntry>
		{
			/// <summary>
			/// Hash code of the documentation tag
			/// </summary>
			internal readonly int HashCode;
			
			/// <summary>
			/// Position in the .xml file where the documentation starts
			/// </summary>
			internal readonly int PositionInFile;
			
			internal IndexEntry(int hashCode, int positionInFile)
			{
				this.HashCode = hashCode;
				this.PositionInFile = positionInFile;
			}
			
			public int CompareTo(IndexEntry other)
			{
				return this.HashCode.CompareTo(other.HashCode);
			}
		}
		
		[NonSerialized]
		XmlDocumentationCache cache = new XmlDocumentationCache();
		
		readonly string fileName;
		DateTime lastWriteDate;
		IndexEntry[] index; // SORTED array of index entries
		
		#region Constructor / Redirection support
		/// <summary>
		/// Creates a new XmlDocumentationProvider.
		/// </summary>
		/// <param name="fileName">Name of the .xml file.</param>
		public XmlDocumentationProvider(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete)) {
				using (XmlTextReader xmlReader = new XmlTextReader(fs)) {
					xmlReader.XmlResolver = null; // no DTD resolving
					xmlReader.MoveToContent();
					if (string.IsNullOrEmpty(xmlReader.GetAttribute("redirect"))) {
						this.fileName = fileName;
						ReadXmlDoc(xmlReader);
					} else {
						string redirectionTarget = GetRedirectionTarget(xmlReader.GetAttribute("redirect"));
						if (redirectionTarget != null) {
							Debug.WriteLine("XmlDoc " + fileName + " is redirecting to " + redirectionTarget);
							using (FileStream redirectedFs = new FileStream(redirectionTarget, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete)) {
								using (XmlTextReader redirectedXmlReader = new XmlTextReader(redirectedFs)) {
									this.fileName = redirectionTarget;
									ReadXmlDoc(redirectedXmlReader);
								}
							}
						} else {
							throw new XmlException("XmlDoc " + fileName + " is redirecting to " + xmlReader.GetAttribute("redirect") + ", but that file was not found.");
						}
					}
				}
			}
		}
		
		static string GetRedirectionTarget(string target)
		{
			string programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
			programFilesDir = AppendDirectorySeparator(programFilesDir);
			
			string corSysDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			corSysDir = AppendDirectorySeparator(corSysDir);
			
			return LookupLocalizedXmlDoc(target.Replace("%PROGRAMFILESDIR%", programFilesDir)
			                             .Replace("%CORSYSDIR%", corSysDir));
		}
		
		static string AppendDirectorySeparator(string dir)
		{
			if (dir.EndsWith("\\", StringComparison.Ordinal) || dir.EndsWith("/", StringComparison.Ordinal))
				return dir;
			else
				return dir + Path.DirectorySeparatorChar;
		}
		
		/// <summary>
		/// Given the assembly file name, looks up the XML documentation file name.
		/// Returns null if no XML documentation file is found.
		/// </summary>
		public static string LookupLocalizedXmlDoc(string fileName)
		{
			string xmlFileName = Path.ChangeExtension(fileName, ".xml");
			string currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
			string localizedXmlDocFile = GetLocalizedName(xmlFileName, currentCulture);
			
			Debug.WriteLine("Try find XMLDoc @" + localizedXmlDocFile);
			if (File.Exists(localizedXmlDocFile)) {
				return localizedXmlDocFile;
			}
			Debug.WriteLine("Try find XMLDoc @" + xmlFileName);
			if (File.Exists(xmlFileName)) {
				return xmlFileName;
			}
			if (currentCulture != "en") {
				string englishXmlDocFile = GetLocalizedName(xmlFileName, "en");
				Debug.WriteLine("Try find XMLDoc @" + englishXmlDocFile);
				if (File.Exists(englishXmlDocFile)) {
					return englishXmlDocFile;
				}
			}
			return null;
		}
		
		static string GetLocalizedName(string fileName, string language)
		{
			string localizedXmlDocFile = Path.GetDirectoryName(fileName);
			localizedXmlDocFile = Path.Combine(localizedXmlDocFile, language);
			localizedXmlDocFile = Path.Combine(localizedXmlDocFile, Path.GetFileName(fileName));
			return localizedXmlDocFile;
		}
		#endregion
		
		#region Load / Create Index
		void ReadXmlDoc(XmlTextReader reader)
		{
			lastWriteDate = File.GetLastWriteTimeUtc(fileName);
			// Open up a second file stream for the line<->position mapping
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete)) {
				LinePositionMapper linePosMapper = new LinePositionMapper(fs);
				List<IndexEntry> indexList = new List<IndexEntry>();
				while (reader.Read()) {
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "members":
								ReadMembersSection(reader, linePosMapper, indexList);
								break;
						}
					}
				}
				indexList.Sort();
				this.index = indexList.ToArray();
			}
		}
		
		sealed class LinePositionMapper
		{
			readonly FileStream fs;
			int currentLine = 1;
			
			public LinePositionMapper(FileStream fs)
			{
				this.fs = fs;
			}
			
			public int GetPositionForLine(int line)
			{
				Debug.Assert(line >= currentLine);
				while (line > currentLine) {
					int b = fs.ReadByte();
					if (b < 0)
						throw new EndOfStreamException();
					if (b == '\n') {
						currentLine++;
					}
				}
				return checked((int)fs.Position);
			}
		}
		
		void ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List<IndexEntry> indexList)
		{
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "members") {
							return;
						}
						break;
					case XmlNodeType.Element:
						if (reader.LocalName == "member") {
							int pos = linePosMapper.GetPositionForLine(reader.LineNumber) + Math.Max(reader.LinePosition - 2, 0);
							string memberAttr = reader.GetAttribute("name");
							if (memberAttr != null)
								indexList.Add(new IndexEntry(memberAttr.GetHashCode(), pos));
							reader.Skip();
						}
						break;
				}
			}
		}
		#endregion
		
		#region GetDocumentation
		/// <inheritdoc/>
		public string GetDocumentation(IEntity entity)
		{
			return GetDocumentation(IDStringProvider.GetIDString(entity));
		}
		
		/// <summary>
		/// Get the documentation for the member with the specified documentation key.
		/// </summary>
		public string GetDocumentation(string key)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			
			int hashcode = key.GetHashCode();
			// index is sorted, so we can use binary search
			int m = Array.BinarySearch(index, new IndexEntry(hashcode, 0));
			if (m < 0)
				return null;
			// correct hash code found.
			// possibly there are multiple items with the same hash, so go to the first.
			while (--m >= 0 && index[m].HashCode == hashcode);
			// m is now 1 before the first item with the correct hash
			
			XmlDocumentationCache cache = this.cache;
			lock (cache) {
				string val = cache.Get(key);
				if (val == null) {
					// go through all items that have the correct hash
					while (++m < index.Length && index[m].HashCode == hashcode) {
						val = LoadDocumentation(key, index[m].PositionInFile);
						if (val != null)
							break;
					}
					// cache the result (even if it is null)
					cache.Add(key, val);
				}
				return val;
			}
		}
		#endregion
		
		#region Load / Read XML
		string LoadDocumentation(string key, int positionInFile)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete)) {
				fs.Position = positionInFile;
				using (XmlTextReader r = new XmlTextReader(fs, XmlNodeType.Element, null)) {
					r.XmlResolver = null; // no DTD resolving
					while (r.Read()) {
						if (r.NodeType == XmlNodeType.Element) {
							string memberAttr = r.GetAttribute("name");
							if (memberAttr == key) {
								return r.ReadInnerXml();
							} else {
								return null;
							}
						}
					}
					return null;
				}
			}
		}
		#endregion
		
		public virtual void OnDeserialization(object sender)
		{
			cache = new XmlDocumentationCache();
		}
	}
}
