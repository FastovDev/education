namespace LinkedList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LinkedListBasic<string>  list = new LinkedListBasic<string>();
            list.Add("one");
            list.Add("two");
            list.AppendFirst("zero");
        }
    }
}
