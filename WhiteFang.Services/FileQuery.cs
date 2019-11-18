using System.Collections.ObjectModel;

namespace WhiteFang.Services
{
    public class FileQuery
    {
        public string Path { get; set; }
        public readonly ObservableCollection<string> Output;

        public FileQuery(string path)
        {
            Path = path;
            Output = new ObservableCollection<string>();
        }
    }
}
