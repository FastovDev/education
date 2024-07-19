using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFO
{
    internal class QueueNode<T>
    {
        public QueueNode<T>? Next { get; set; }
        public T Value { get; set; }
        public QueueNode(T value) 
        {
            Value = value;
        }
    }
}
