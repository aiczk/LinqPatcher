using System.Diagnostics;
using System.Linq;
using LinqPatcher.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Script
{
    public class Foo : MonoBehaviour
    {
        private Baa[] array = new Baa[10000];
        
        [Optimize]
        private void Optimize(Baa[] arr)
        {
            var num = arr.Select(x => x.Index).Where(x => x % 2 == 0).Select(x => x * 2).OrderBy(x => x < 42);
        }
        
        private void NoOptimize()
        {
            var num = array.Select(x => x.Index).Where(x => x % 2 == 0).Select(x => x * 2);
        }

        //[Optimize]
        private void FFA(Baa[] arr)
        {
            var s = arr.Select(x => x.Index);
        }

        private void Start()
        {
            InitArray();

            var stopWatch = new Stopwatch();
            
            stopWatch.Start();
            NoOptimize();
            stopWatch.Stop();
            Debug.Log($"Non : {stopWatch.Elapsed.ToString()}");
            
            stopWatch.Restart();
            Optimize(array);
            stopWatch.Stop();
            Debug.Log($"Opt : {stopWatch.Elapsed.ToString()}");
        }

        private void InitArray()
        {
            for (var i = 0; i < array.Length; i++) 
                array[i] = new Baa(i);
        }
    }

    public class Baa
    {
        public int Index { get; }
        
        public Baa(int index)
        {
            Index = index;
        }

    }
}
