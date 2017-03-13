namespace OSPSuite.Utility.Compression
{
   public interface ICompression
   {
      byte[] Compress(byte[] byteArrayToCompress);
      byte[] Decompress(byte[] byteArrayToDecompress);
   }
}