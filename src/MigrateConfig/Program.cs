﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Cradiator.MigrateConfig
{
	class Program
	{
		// a quick, manually tested conversion tool, to convert old app.config format to new
		// specifically to remove URL/ProjectRegEx and replace with <views><view url=""...>
		static int Main(string[] args)
		{
			// check for app settings
			var xmlFile = args.Length == 1 ? args[0] : "Cradiator.exe.config";

			if (!File.Exists(xmlFile))
			{
				Console.WriteLine("xml config file doesn't exist: '{0}'", xmlFile);
				return 1;
			}

			var xdoc = XDocument.Load(xmlFile);
			var addElements = 
				xdoc.Element("configuration")
					.Element("appSettings")
					.Elements("add");

			var oldElements = from element in addElements
			                  let e = element.Attribute("key").Value
			                  where e == "URL" || 
									e == "Skin" ||
									e == "ProjectNameRegEx" ||
									e == "CategoryRegEx"
			                  select element;

			if (!oldElements.Any())
			{
				Console.WriteLine("no pre multi-view config detected");
				return 0;
			}

			var config = new Dictionary<string, string>();
			Console.WriteLine("removing pre multi-view xml elements...");
			foreach (var oldElement in oldElements)
			{
				config[oldElement.Attribute("key").Value] = oldElement.Attribute("value").Value;
			}

			oldElements.Remove();

			Console.WriteLine("Adding multi-view xml elements...");

			//TODO this has to go after configuration section
			xdoc.Element("configuration").AddFirst(
				new XElement("views",
				new XElement("view",
				            new XAttribute("url", config["URL"]),
				            new XAttribute("skin", config["Skin"]),
				            new XAttribute("project-regex", config["ProjectNameRegEx"]),
				            new XAttribute("category-regex", config["CategoryRegEx"])
				)));

			xdoc.Element("configuration")
				.Element("configSections").Add(
					new XElement("section",
						new XAttribute("type", "System.Configuration.IgnoreSectionHandler"),
						new XAttribute("name", "views")
			));

			xdoc.Save(xmlFile);

			return 0;
		}
	}
}