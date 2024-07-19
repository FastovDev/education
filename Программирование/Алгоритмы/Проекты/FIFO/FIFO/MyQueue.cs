using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFO
{
    internal class MyQueue<T> : IEnumerable<T>, IEnumerable
    {
        int count;
        public QueueNode<T>? FirstNode { get; set; }
        public QueueNode<T>? LastNode { get; set; }
        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        public void Enqueue(T value)
        {
            QueueNode<T>? newNode = new QueueNode<T>(value);
            QueueNode<T>? oldLastNode = LastNode;
            LastNode = newNode;

            if (count == 0)
                FirstNode = LastNode;
            else
                oldLastNode!.Next = LastNode;

            count++;
        }

        public T? Dequeue()
        {
            if (count == 0)
                return default;

            T firstNodeValue = FirstNode!.Value;
            FirstNode = FirstNode.Next;
            count--;

            return firstNodeValue;
        }

        public T? Peek()
        {
            if (count == 0)
                return default;

            return FirstNode!.Value;
        }

        public void Clear()
        {
            FirstNode = null;
            LastNode = null;
            count = 0;
        }

        public bool Contains(T data)
        {
            QueueNode<T>? current = FirstNode;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(data))
                    return true;
                current = current.Next;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            QueueNode<T>? current = FirstNode;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
    }
}
