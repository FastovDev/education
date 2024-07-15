using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedList
{
    internal class LinkedListDoubly<T> : IEnumerable<T>, IEnumerable
    {
        LinkedListDoublyObject<T>? First;
        LinkedListDoublyObject<T>? Last;
        int count;

        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        public T? GetValue(T value)
        {
            LinkedListDoublyObject<T>? current = First;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value))
                    return value;

                current = current.Next;
            }
            return default;
        }

        public T? GetValue(LinkedListDoublyObject<T> linkedListObject)
        {
            return GetValue(linkedListObject.Value);
        }

        public void Add(T value)
        {
            LinkedListDoublyObject<T>? linkedListObject = new LinkedListDoublyObject<T>(value);

            if (First is null)
                First = linkedListObject;
            else
                Last!.Next = linkedListObject;

            Last = linkedListObject;
            count++;
        }
        public void Add(LinkedListDoublyObject<T> linkedListObject)
        {
            Add(linkedListObject.Value);
        }

        public void AppendFirst(T value)
        {
            LinkedListDoublyObject<T>? linkedListObject = new LinkedListDoublyObject<T>(value);
            linkedListObject.Next = First;
            First = linkedListObject;

            if (count == 0)
                Last = First;

            count++;
        }

        public void AppendFirst(LinkedListDoublyObject<T> linkedListObject)
        {
            AppendFirst(linkedListObject.Value);
        }

        public bool Contains(T value)
        {
            LinkedListDoublyObject<T>? current = First;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value))
                    return true;

                current = current.Next;
            }
            return false;
        }

        public bool Contains(LinkedListDoublyObject<T> linkedListObject)
        {
            return Contains(linkedListObject.Value);
        }

        public bool Remove(T value)
        {
            LinkedListDoublyObject<T>? current = First;
            LinkedListDoublyObject<T>? previous = null;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value))
                {
                    if (previous is null)
                        First = current.Next;

                    if (First is null)
                        Last = null;

                    if (current.Next is null)
                        Last = previous;

                    count--;
                    return true;
                }
                previous = current;
                current = current.Next;
            }
            return false;
        }
        public bool Remove(LinkedListDoublyObject<T> linkedListObject)
        {
            return Remove(linkedListObject.Value);
        }

        public bool Update(T oldValue, T newValue)
        {
            LinkedListDoublyObject<T>? current = First;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(oldValue))
                {
                    current.Value = newValue;
                    return true;
                }

                current = current.Next;
            }
            return false;
        }

        public bool Update(LinkedListDoublyObject<T> oldLinkedListObject, LinkedListDoublyObject<T> newLinkedListObject)
        {
            return Update(oldLinkedListObject.Value, newLinkedListObject.Value);
        }

        public void Clear()
        {
            First = null;
            Last = null;
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            LinkedListDoublyObject<T>? current = First;

            while (current!.Next is not null)
            {
                yield return current!.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
