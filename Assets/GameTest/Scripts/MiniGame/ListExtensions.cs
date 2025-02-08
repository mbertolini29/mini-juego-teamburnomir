using System;
using System.Collections;
using System.Collections.Generic;

namespace Test
{
    public static class ListExtensions 
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            //Random Number Generator
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);

                /* se utilizo tupla para intercambiar valores en un sola línea.
                 * T temp = list[k];
                 * list[k] = list[n];
                 * list[n] = temp; 
                 */
            }
        }
    }
}
