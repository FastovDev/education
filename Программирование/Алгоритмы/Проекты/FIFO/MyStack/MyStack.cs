using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStack
{
    internal class MyStack<T>
    {
        private T?[] collection;
        int count;
        public MyStack(int sizeOfCollection = 10) 
        {
            collection = new T[sizeOfCollection];
        }
        public bool IsEmpty
        {
            get { return count == 0; }
        }
        public int Count
        {
            get { return count; }
        }
        public void Push(T item)
        {
            if (count == collection.Length)
            {
                T[] newCollection = new T[collection.Length * 2];
                for (int i =0; i< collection.Length;i++)
                    newCollection[i] = collection[i]!;
                collection = newCollection;
            }
                
            collection[count++] = item;
        }
        public T? Pop()
        {
            if (IsEmpty)
                return default;
            T item = collection[--count]!;
            collection[count] = default;
            return item;
        }

        public T? Peek()
        {
            if (IsEmpty)
                return default;
            return collection[count - 1];
        }
    }
}
