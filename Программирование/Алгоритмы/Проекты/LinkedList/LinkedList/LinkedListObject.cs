namespace LinkedList
{
    internal class LinkedListObject<T>
    {
        public T Value { get; set; }
        public LinkedListObject<T>? Next { get; set; }
        public LinkedListObject(T value) 
        {
            Value = value;
        }
    }    
}
