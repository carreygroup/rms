using System;
using System.Xml;
using System.IO;
using System.Collections;

namespace RMSPPC
{
	/// <summary>
	/// Setting 的摘要说明。
	/// </summary>
	public class Setting
	{
		Hashtable _list = new Hashtable();
		string _filePath;

		public Setting()
		{
			_filePath=GetFilePath();
		}

		public string GetWebUrl()
		{
			_list.Clear();
			if (File.Exists(_filePath))
			{
				XmlTextReader reader = new XmlTextReader(_filePath);
				while (reader.Read())
				{
					if ((reader.NodeType == XmlNodeType.Element)&&(reader.Name == "add"))
					{
						_list[reader.GetAttribute("key")] = reader.GetAttribute("value");
					}
				}
				reader.Close();
			}
			string rtn = (string)(_list["WebUrl"]);
			return rtn;
		}

		private string GetFilePath()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase + ".config";
		}

	}
}
