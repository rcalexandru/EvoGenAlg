using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoGenAlg
{
    public interface IEvolution
    {
        void Initialize();
        IEvolution Mutate();
        void Evolve();
        int Fitness{ get; set; }
        int DesiredFitness { get; }
        void Serialize();
        object Data { get; set; }
    }

    public class EvoComparer : IComparer<IEvolution>
    {
        public int Compare(IEvolution x, IEvolution y)
        {
            if (x.Fitness < y.Fitness)
                return 1;
            if (x.Fitness > y.Fitness)
                return -1;
            else
                return 0;
        }
    }

}
