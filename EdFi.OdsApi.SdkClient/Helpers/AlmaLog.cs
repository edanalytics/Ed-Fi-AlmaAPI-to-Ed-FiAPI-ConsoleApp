using System.IO;

namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
    public interface IAlmaLog
    {
        void Save(string Resource, string FileName, string srcAlmaEndPoint, string srcError);
    }
    public class AlmaLog : IAlmaLog
    {
        public void Save(string Resource, string FileName, string srcAlmaEndPoint, string srcError)
        {
            string fileName = $@"C:\AlmaCsvErrors\{Resource}-{FileName}.log";
            TextWriter tw = File.AppendText(fileName);
            var errorFormat = $"{Resource},{srcAlmaEndPoint},{srcError}";
            tw.WriteLine(errorFormat);
            tw.Close();
        }
    }
}