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
            list.Remove("two");
            list.Contains("one");
            list.Contains("two");
            list.Remove("zero");
            list.Remove("one");

            LinkedListDoubly<string> listDoubly = new LinkedListDoubly<string>();
            listDoubly.Add("one");
            listDoubly.Add("two");
            listDoubly.AppendFirst("zero");
            listDoubly.Remove("two");
            listDoubly.Contains("one");
            listDoubly.Contains("two");
            listDoubly.Remove("zero");
            listDoubly.Remove("one");

            LinkedListCircle<string> listDoublyCircle = new LinkedListCircle<string>();
            listDoublyCircle.Add("one");
            listDoublyCircle.Add("two");
            listDoublyCircle.AppendFirst("zero");
            listDoublyCircle.Remove("two");
            listDoublyCircle.Contains("one");
            listDoublyCircle.Contains("two");
            listDoublyCircle.Remove("zero");
            listDoublyCircle.Remove("one");
        }
    }
}
