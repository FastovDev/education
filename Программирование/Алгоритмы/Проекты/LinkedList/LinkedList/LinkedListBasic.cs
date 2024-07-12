using System.Collections;

namespace LinkedList
{
    internal class LinkedListBasic<T> : IEnumerable<T>, IEnumerable
    {
        LinkedListObject<T>? First;
        LinkedListObject<T>? Last;
        int count;

        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        public T? GetValue(T value)
        {
            LinkedListObject<T>? current = First;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value))
                    return value;

                current = current.Next;
            }
            return default;
        }

        public T? GetValue(LinkedListObject<T> linkedListObject)
        {
            return GetValue(linkedListObject.Value);
        }

        public void Add(T value)
        {
            LinkedListObject<T>? linkedListObject = new LinkedListObject<T>(value);

            if (First is null)
                First = linkedListObject;
            else
                Last!.Next = linkedListObject;

            Last = linkedListObject;
            count++;
        }
        public void Add(LinkedListObject<T> linkedListObject)
        {
            Add(linkedListObject.Value);
        }
        
        public void AppendFirst(T value)
        {
            LinkedListObject<T>? linkedListObject = new LinkedListObject<T>(value);
            linkedListObject.Next = First;
            First = linkedListObject;

            if (count == 0)
                Last = First;

            count++;
        }

        public void AppendFirst(LinkedListObject<T> linkedListObject)
        {
            AppendFirst(linkedListObject.Value);
        }

        public bool Contains(T value)
        {
            LinkedListObject<T>? current = First;
            while (current is not null && current.Value is not null)
            {
                if (current.Value.Equals(value)) 
                    return true;

                current = current.Next;
            }
            return false;
        }

        public bool Contains(LinkedListObject<T> linkedListObject)
        {
            return Contains(linkedListObject.Value);
        }
       
        public bool Remove(T value)
        {
            LinkedListObject<T>? current = First;
            LinkedListObject<T>? previous = null;
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
        public bool Remove(LinkedListObject<T> linkedListObject)
        {
            return Remove(linkedListObject.Value);
        }

        public bool Update(T oldValue, T newValue)
        {
            LinkedListObject<T>? current = First;
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

        public bool Update(LinkedListObject<T> oldLinkedListObject, LinkedListObject<T> newLinkedListObject)
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
            LinkedListObject<T>? current = First;

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
