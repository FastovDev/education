namespace MyStack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyStack<string> stack = new MyStack<string>();
            stack.Push("a");
            stack.Push("b");
            stack.Push("c");
            stack.Push("d");
            stack.Push("e");
            stack.Push("f");                
            stack.Push("g");
            stack.Push("h");
            Console.WriteLine(stack.Count);
            stack.Push("i");
            stack.Push("j");
            stack.Push("k");
            stack.Push("l");
            Console.WriteLine(stack.Count);
            Console.WriteLine(stack.Peek());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop()); 
            Console.WriteLine(stack.Pop()); 
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
        }
    }
}
