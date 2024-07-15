using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedList
{
    internal class LinkedListDoublyObject<T>
    {
        public T Value { get; set; }
        public LinkedListDoublyObject<T>? Prev { get; set; }
        public LinkedListDoublyObject<T>? Next { get; set; }
        public LinkedListDoublyObject(T value)
        {
            Value = value;
        }
    }
}
