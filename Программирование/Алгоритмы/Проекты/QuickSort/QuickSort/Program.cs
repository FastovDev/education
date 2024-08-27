using System.Runtime.CompilerServices;

namespace QuickSort
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            int[] randomMassive = new int[20];
            for (int i = 0; i < randomMassive.Length; i++)
            {
                randomMassive[i] = rnd.Next(0,100);
            }
            Console.WriteLine("Unsorted");
            foreach (int i in randomMassive)
            {
                
                Console.Write(i + " ");
            }
            QuickSort.Sort(randomMassive);
            Console.WriteLine("\nSorted");
            foreach (int i in randomMassive)
            {                
                Console.Write(i + " ");
            }
        }
    }
    static class QuickSort
    {
        //метод для обмена элементов массива
        static void Swap(ref int x, ref int y)
        {
            var t = x;
            x = y;
            y = t;
        }

        //метод возвращающий индекс опорного элемента
        static int Partition(int[] array, int minIndex, int maxIndex)
        {
            var pivot = minIndex - 1;
            for (var i = minIndex; i < maxIndex; i++)
            {
                if (array[i] < array[maxIndex])
                {
                    pivot++;
                    Swap(ref array[pivot], ref array[i]);
                }
            }

            pivot++;
            Swap(ref array[pivot], ref array[maxIndex]);
            return pivot;
        }

        //быстрая сортировка
        static int[] Sort(int[] array, int minIndex, int maxIndex)
        {
            if (minIndex >= maxIndex)
            {
                return array;
            }

            var pivotIndex = Partition(array, minIndex, maxIndex);
            Sort(array, minIndex, pivotIndex - 1);
            Sort(array, pivotIndex + 1, maxIndex);

            return array;
        }

        public static int[] Sort(int[] array)
        {
            return Sort(array, 0, array.Length - 1);
        }
    }
    
}
