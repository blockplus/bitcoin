﻿using System;
using System.Text;

namespace DotNetWallet.Helpers
{
	public class PasswordConsole
    {
		/// <summary>
		/// Gets the console password.
		/// </summary>
		/// <returns></returns>
		internal static string ReadPassword()
		{
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				ConsoleKeyInfo cki = Console.ReadKey(true);
				if (cki.Key == ConsoleKey.Enter)
				{
					Console.WriteLine();
					break;
				}

				if (cki.Key == ConsoleKey.Backspace)
				{
					if (sb.Length > 0)
					{
						Console.Write("\b\0\b");
						sb.Length--;
					}

					continue;
				}

				Console.Write('*');
				sb.Append(cki.KeyChar);
			}

			return sb.ToString();
		}
	}
}
