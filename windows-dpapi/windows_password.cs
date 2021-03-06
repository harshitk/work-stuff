using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class MemoryProtection
{
    public static void Main()
    {
        Run();
    }

    public static void Run()
    {
        try
        {
            // Create the original data to be encrypted (The data length should be a multiple of 16).
            byte[] toEncrypt = UnicodeEncoding.ASCII.GetBytes("ThisIsSomeData16");

            ///////////////////////////////
            //
            // Data Encryption - ProtectedData - Encrypt
            //
            ///////////////////////////////

            // Create a file.
            FileStream fStream = new FileStream("Data.dat", FileMode.OpenOrCreate);

            // Create some random entropy.
			// byte[] entropy = {1,2,3,4,5,6};
			
            byte[] entropy = CreateRandomEntropy(); 

            Console.WriteLine();
            Console.WriteLine("Original data: " + UnicodeEncoding.ASCII.GetString(toEncrypt));
            Console.WriteLine("Encrypting and writing to disk...");

            // Encrypt a copy of the data to the stream.
            int bytesWritten = EncryptDataToStream(toEncrypt, entropy, DataProtectionScope.CurrentUser, fStream);

            fStream.Close();

			///////////////////////////////
            //
            // Data Encryption - ProtectedData - Decrypt
            //
            ///////////////////////////////
			
			
            Console.WriteLine("Reading data from disk and decrypting...");

            // Open the file.
            fStream = new FileStream("Data.dat", FileMode.Open);

            // Read from the stream and decrypt the data.
            byte[] decryptData = DecryptDataFromStream(entropy, DataProtectionScope.CurrentUser, fStream, bytesWritten);

            fStream.Close();
			
            Console.WriteLine("Decrypted data: " + UnicodeEncoding.ASCII.GetString(decryptData));

        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }

    }

    public static byte[] CreateRandomEntropy()
    {
        // Create a byte array to hold the random value.
        byte[] entropy = new byte[16];

        // Create a new instance of the RNGCryptoServiceProvider.
        // Fill the array with a random value.
        new RNGCryptoServiceProvider().GetBytes(entropy);

        // Return the array.
        return entropy;


    }

    public static int EncryptDataToStream(byte[] Buffer, byte[] Entropy, DataProtectionScope Scope, Stream S)
    {
        if (Buffer.Length <= 0)
            throw new ArgumentException("Buffer");
        if (Buffer == null)
            throw new ArgumentNullException("Buffer");
        if (Entropy.Length <= 0)
            throw new ArgumentException("Entropy");
        if (Entropy == null)
            throw new ArgumentNullException("Entropy");
        if (S == null)
            throw new ArgumentNullException("S");
       
        int length = 0;

        // Encrypt the data in memory. The result is stored in the same same array as the original data.
        byte[] encrptedData = ProtectedData.Protect(Buffer, Entropy, Scope);

        // Write the encrypted data to a stream.
        if (S.CanWrite && encrptedData != null)
        {
            S.Write(encrptedData, 0, encrptedData.Length);

            length = encrptedData.Length;
        }
		Console.WriteLine("Encrypted data: " + UnicodeEncoding.ASCII.GetString(toEncrypt));
		
        // Return the length that was written to the stream. 
        return length;
        
    }

    public static byte[] DecryptDataFromStream(byte[] Entropy, DataProtectionScope Scope, Stream S, int Length)
    {
        if (S == null)
            throw new ArgumentNullException("S");
        if (Length <= 0 )
            throw new ArgumentException("Length");
        if (Entropy == null)
            throw new ArgumentNullException("Entropy");
        if (Entropy.Length <= 0)
            throw new ArgumentException("Entropy");
        

        
        byte[] inBuffer = new byte[Length];
        byte[] outBuffer;

        // Read the encrypted data from a stream.
        if (S.CanRead)
        {
            S.Read(inBuffer, 0, Length);

            outBuffer = ProtectedData.Unprotect(inBuffer, Entropy, Scope);
        }
        else
        {
            throw new IOException("Could not read the stream.");
        }

        // Return the length that was written to the stream. 
        return outBuffer;

    }


}
