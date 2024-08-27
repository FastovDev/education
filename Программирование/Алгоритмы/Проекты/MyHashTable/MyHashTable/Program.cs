using System.Collections;

namespace MyHashTable
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Hashtable table = new Hashtable();
            table.Add("key", "value");
            Console.WriteLine(table.Values);

        }
    }
}
