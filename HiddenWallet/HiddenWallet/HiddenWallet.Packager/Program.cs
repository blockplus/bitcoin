﻿using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HiddenWallet.Packager
{
    class Program
    {
        static void Main(string[] args)
		{
			var packagerProjectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
			var apiProjectDirectory = Path.Combine(packagerProjectDirectory, "..\\HiddenWallet.API");
			var guiProjectDirectory = Path.Combine(packagerProjectDirectory, "..\\HiddenWallet.GUI");
			var solutionDirectory = Path.Combine(packagerProjectDirectory, "..\\..\\");

			var packageJsonString = File.ReadAllText(Path.Combine(guiProjectDirectory, "package.json"));
			JToken packageJson = JObject.Parse(packageJsonString);
			var version = packageJson.SelectToken("version").Value<string>();

			// https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog
			string[] targets =
			{
				"win7-x64",
				"win8-x64",
				"win81-x64",
				"win10-x64",
			};
			UpdateCsproj(apiProjectDirectory, targets);

			var psiBuild = new ProcessStartInfo
			{
				FileName = "dotnet",
				Arguments = "build",
				WorkingDirectory = apiProjectDirectory
			};
			var pBuild = Process.Start(psiBuild);
			pBuild.WaitForExit();

			foreach (var target in targets)
			{
				var currDistDir = Path.Combine(apiProjectDirectory, "bin\\dist", target);
				if (Directory.Exists(currDistDir))
				{
					DeleteDirectoryRecursively(currDistDir);
				}
				Directory.CreateDirectory(currDistDir);

				var torFolderPath = Path.Combine(currDistDir, "tor");
				Console.WriteLine("Replacing tor...");
				if (Directory.Exists(torFolderPath))
				{
					DeleteDirectoryRecursively(torFolderPath);
				}
				try
				{
					ZipFile.ExtractToDirectory(Path.Combine(packagerProjectDirectory, "tor.zip"), currDistDir);
				}
				catch(UnauthorizedAccessException)
				{
					Task.Delay(100);
					ZipFile.ExtractToDirectory(Path.Combine(packagerProjectDirectory, "tor.zip"), currDistDir);
				}

				var psiPublish = new ProcessStartInfo
				{
					FileName = "dotnet",
					Arguments = $"publish -r {target} --output bin/dist/{target}",
					WorkingDirectory = apiProjectDirectory
				};
				var pPublish = Process.Start(psiPublish);
				pPublish.WaitForExit();
			}

			var distDir = Path.Combine(solutionDirectory, "dist");
			if (Directory.Exists(distDir))
			{
				DeleteDirectoryRecursively(distDir);
			}

			var psiNpmRunDist = new ProcessStartInfo
			{
				FileName = "cmd",
				RedirectStandardInput = true,
				WorkingDirectory = guiProjectDirectory
			};
			var pNpmRunDist = Process.Start(psiNpmRunDist);
			pNpmRunDist.StandardInput.WriteLine("npm run pack & exit");
			pNpmRunDist.WaitForExit();

			foreach (var file in Directory.GetFiles(distDir))
			{
				if (file.EndsWith(".exe")) File.Delete(file);
			}

			foreach(var target in targets)
			{
				Console.WriteLine($"Preparing final package for {target}");
				string targetWithoutArch = target.Remove(target.Length - 4);

				string currentDistributionDirectory = Path.Combine(distDir, "HiddenWallet-" + version + "-" + targetWithoutArch);
				CloneDirectory(Path.Combine(distDir, "win-unpacked"), currentDistributionDirectory);
				string currTargDir = Path.Combine(currentDistributionDirectory, "resources\\HiddenWallet.API\\bin\\dist\\current-target");
				Directory.CreateDirectory(currTargDir);
				var apiTargetDir = Path.Combine(apiProjectDirectory, "bin\\dist", target);
				CloneDirectory(apiTargetDir, currTargDir);

				ZipFile.CreateFromDirectory(currentDistributionDirectory, currentDistributionDirectory + ".zip", CompressionLevel.Optimal, true);
				DeleteDirectoryRecursively(currentDistributionDirectory);
			}

			DeleteDirectoryRecursively(Path.Combine(distDir, "win-unpacked"));

			Console.WriteLine("Finished. Press key to exit...");
			Console.ReadKey();
		}

		private static void DeleteDirectoryRecursively(string directory)
		{
			try
			{
				DeleteDirectoryIfExists(directory, true);
			}
			catch (IOException)
			{
				Task.Delay(100);
				if (Directory.Exists(directory))
				{
					DeleteDirectoryIfExists(directory, true);
				}
			}
		}

		private static void DeleteDirectoryIfExists(string directory, bool recursive)
		{
			if (Directory.Exists(directory))
			{
				try
				{
					Directory.Delete(directory, recursive);
				}
				catch (DirectoryNotFoundException)
				{
					// for some reason it still happen
				}
			}
		}

		private static void CloneDirectory(string root, string dest)
		{
			foreach (var directory in Directory.GetDirectories(root))
			{
				string dirName = Path.GetFileName(directory);
				if (!Directory.Exists(Path.Combine(dest, dirName)))
				{
					Directory.CreateDirectory(Path.Combine(dest, dirName));
				}
				CloneDirectory(directory, Path.Combine(dest, dirName));
			}

			foreach (var file in Directory.GetFiles(root))
			{
				File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
			}
		}

		private static void UpdateCsproj(string apiProjectDirectory, string[] targets)
		{
			string csprojFile = Path.Combine(apiProjectDirectory, "HiddenWallet.API.csproj");
			var csprojString = File.ReadAllText(csprojFile);
			var csprojXml = new XmlDocument();
			csprojXml.LoadXml(csprojString);
			var csprojTargets = csprojXml.GetElementsByTagName("RuntimeIdentifiers")[0].InnerText.Split(';').ToList();
			var added = false;
			foreach (var target in targets)
			{
				if (!csprojTargets.Contains(target))
				{
					csprojTargets.Add(target);
					added = true;
				}
			}
			if (added)
			{
				csprojXml.GetElementsByTagName("RuntimeIdentifiers")[0].InnerText = string.Join(";", csprojTargets);
				using (var fs = new FileStream(csprojFile, FileMode.Create))
				{
					csprojXml.Save(fs);
				}
			}
		}
	}
}