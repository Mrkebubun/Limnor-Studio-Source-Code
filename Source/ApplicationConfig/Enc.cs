/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.IO;
using System.Security.Cryptography;

namespace Limnor.Application
{
	internal sealed class SimpleEncryption
	{
		private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
		{
			MemoryStream ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = Key;
			alg.IV = IV;
			CryptoStream cs = new CryptoStream(ms,
			   alg.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(clearData, 0, clearData.Length);
			cs.Close();
			byte[] encryptedData = ms.ToArray();
			return encryptedData;
		}
		public static string Encrypt(string clearText, string Password)
		{
			byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
			byte[] encryptedData = Encrypt(clearBytes,
					 pdb.GetBytes(32), pdb.GetBytes(16));
			return Convert.ToBase64String(encryptedData);
		}
		private static byte[] Encrypt(byte[] clearData, string Password)
		{
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
			return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
		}
		public static void Encrypt(string fileIn,
					string fileOut, string Password)
		{
			// First we are going to open the file streams 
			FileStream fsIn = new FileStream(fileIn,
				FileMode.Open, FileAccess.Read);
			FileStream fsOut = new FileStream(fileOut,
				FileMode.OpenOrCreate, FileAccess.Write);

			// Then we are going to derive a Key and an IV from the
			// Password and create an algorithm 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

			Rijndael alg = Rijndael.Create();
			alg.Key = pdb.GetBytes(32);
			alg.IV = pdb.GetBytes(16);

			// Now create a crypto stream through which we are going
			// to be pumping data. 
			// Our fileOut is going to be receiving the encrypted bytes. 
			CryptoStream cs = new CryptoStream(fsOut,
				alg.CreateEncryptor(), CryptoStreamMode.Write);

			// Now will will initialize a buffer and will be processing
			// the input file in chunks. 
			// This is done to avoid reading the whole file (which can
			// be huge) into memory. 
			int bufferLen = 4096;
			byte[] buffer = new byte[bufferLen];
			int bytesRead;

			do
			{
				// read a chunk of data from the input file 
				bytesRead = fsIn.Read(buffer, 0, bufferLen);

				// encrypt it 
				cs.Write(buffer, 0, bytesRead);
			} while (bytesRead != 0);

			// close everything 

			// this will also close the unrelying fsOut stream
			cs.Close();
			fsIn.Close();
		}

		private static byte[] Decrypt(byte[] cipherData,
									byte[] Key, byte[] IV)
		{
			MemoryStream ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = Key;
			alg.IV = IV;
			CryptoStream cs = new CryptoStream(ms,
				alg.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(cipherData, 0, cipherData.Length);
			cs.Close();
			byte[] decryptedData = ms.ToArray();
			return decryptedData;
		}

		public static string Decrypt(string cipherText, string Password)
		{
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 
            0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
			byte[] decryptedData = Decrypt(cipherBytes,
				pdb.GetBytes(32), pdb.GetBytes(16));
			return System.Text.Encoding.Unicode.GetString(decryptedData);
		}

		private static byte[] Decrypt(byte[] cipherData, string Password)
		{
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
			return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
		}

		// Decrypt a file into another file using a password 
		public static void Decrypt(string fileIn,
					string fileOut, string Password)
		{

			// First we are going to open the file streams 
			FileStream fsIn = new FileStream(fileIn,
						FileMode.Open, FileAccess.Read);
			FileStream fsOut = new FileStream(fileOut,
						FileMode.OpenOrCreate, FileAccess.Write);

			// Then we are going to derive a Key and an IV from
			// the Password and create an algorithm 
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
				new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
			Rijndael alg = Rijndael.Create();

			alg.Key = pdb.GetBytes(32);
			alg.IV = pdb.GetBytes(16);

			// Now create a crypto stream through which we are going
			// to be pumping data. 
			// Our fileOut is going to be receiving the Decrypted bytes. 
			CryptoStream cs = new CryptoStream(fsOut,
				alg.CreateDecryptor(), CryptoStreamMode.Write);

			// Now will will initialize a buffer and will be 
			// processing the input file in chunks. 
			// This is done to avoid reading the whole file (which can be
			// huge) into memory. 
			int bufferLen = 4096;
			byte[] buffer = new byte[bufferLen];
			int bytesRead;

			do
			{
				// read a chunk of data from the input file 
				bytesRead = fsIn.Read(buffer, 0, bufferLen);

				// Decrypt it 
				cs.Write(buffer, 0, bytesRead);

			} while (bytesRead != 0);

			// close everything 
			cs.Close(); // this will also close the unrelying fsOut stream 
			fsIn.Close();
		}
	}
}