namespace Bps.uCloud.Contracts
{
    public class Data
    {
        public byte[] Bytes { get; set; }

        public string Format { get; set; }

        public int Size { get; set; }

        public DataTypes Type { get; set; }

        public Data()
        {
            Format = string.Empty;
            Type = DataTypes.None;
        }

        public Data(string format, DataTypes type)
        {
            Format = format;
            Type = type;
        }
    }
}