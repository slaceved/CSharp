using System;
using System.Collections.Generic;

    internal class Quicksort
    {
        private static void Swap(IList<int> array, int left, int right)
        {
            var temp = array[right];
            array[right] = array[left];
            array[left] = temp;
        }

        public void Sort(int[] array, int left, int right)
        {
            while (true)
            {
                var lHold = left;
                var rHold = right;
                var objRan = new Random();
                var pivot = objRan.Next(left, right);
                Swap(array, pivot, left);
                pivot = left;
                left++;

                while (right >= left)
                {
                    if (array[left] >= array[pivot] && array[right] < array[pivot])
                        Swap(array, left, right);
                    else if (array[left] >= array[pivot])
                        right--;
                    else if (array[right] < array[pivot])
                        left++;
                    else
                    {
                        right--;
                        left++;
                    }
                }
                Swap(array, pivot, right);
                pivot = right;
                if (pivot > lHold)
                    Sort(array, lHold, pivot);
                if (rHold > pivot + 1)
                {
                    left = pivot + 1;
                    right = rHold;
                    continue;
                }
                break;
            }
        }
    }