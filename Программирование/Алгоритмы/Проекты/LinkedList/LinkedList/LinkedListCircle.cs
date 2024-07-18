using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LinkedList
{
    internal class LinkedListCircle<T> : IEnumerable<T>, IEnumerable
    {
        LinkedListDoublyObject<T>? First;
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

        public T? GetValueReverse(T value)
        {
            LinkedListDoublyObject<T>? current = First;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value))
                    return value;

                current = current.Prev;
            }
            return default;
        }

        public T? GetValueReverse(LinkedListDoublyObject<T> linkedListObject)
        {
            return GetValueReverse(linkedListObject.Value);
        }

        public void Add(T value)
        {
            LinkedListDoublyObject<T>? linkedListObject = new LinkedListDoublyObject<T>(value);

            if (First is null)
            {
                First = linkedListObject;
                First.Next = First;
                First.Prev = First;
            }
            else
            {
                linkedListObject.Next = First;
                linkedListObject.Prev = First.Prev;
                First.Prev!.Next = linkedListObject;
                First.Prev = linkedListObject;
            }

            count++;
        }
        public void Add(LinkedListDoublyObject<T> linkedListObject)
        {
            Add(linkedListObject.Value);
        }

        public void AppendFirst(T value)
        {
            LinkedListDoublyObject<T>? linkedListObject = new LinkedListDoublyObject<T>(value);

            if (First is not null)
            {
                linkedListObject.Next = First;
                linkedListObject.Prev = First.Prev;
                First.Prev!.Next = linkedListObject;
                First.Prev = linkedListObject;
                First = linkedListObject;
            }
            else
                Add(value);                

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
                if (current == First) break;
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

            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value))
                {
                    if (current == First)
                    {
                        First = First.Next;
                    }
                    current!.Next!.Prev = current.Prev;
                    current!.Prev!.Next = current.Next;

                    count--;
                    if (count == 0) Clear();
                    return true;
                }
                current = current.Next;
                if (current == First) return false;
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
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            LinkedListDoublyObject<T>? current = First;

            while (current is not null && current!.Next is not null)
            {
                yield return current!.Value;
                current = current.Next;
                if (current == First) break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
