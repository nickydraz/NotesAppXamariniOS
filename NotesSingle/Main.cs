using UIKit;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NotesSingle
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
 			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
