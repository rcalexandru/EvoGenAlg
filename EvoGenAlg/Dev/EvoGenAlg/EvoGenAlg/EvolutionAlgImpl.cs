using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoGenAlg
{
    public class EvolutionAlgImpl
    {
        IEvolution _evo = null;
        List<IEvolution> _absoluteEvolutions = null;
        int _nMutations;
        int _nFreshMutationsPerGeneration;
        int _nChildrenPerGeneration;
        static EvoComparer _evoComparer = new EvoComparer();
        int _nRefreshCounter;

        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        public EvolutionAlgImpl(IEvolution evo)
        {
            _evo = evo;
            _absoluteEvolutions = new List<IEvolution>();
            _nMutations = 1;
            _nChildrenPerGeneration = 1;
            _nFreshMutationsPerGeneration = 0;
            _nRefreshCounter = 1;
        }

        public void Evolve()
        {
            _evo.Initialize();

            IEvolution mutation = _evo.Mutate();

            ProgressEventArgs args = new ProgressEventArgs(mutation.Data, mutation.Fitness);
            OnUpdateStatus(this, args);

            mutation.Serialize();
        }

        public void Evolve2()
        {
            _evo.Initialize();

            List<IEvolution> mutations = new List<IEvolution>();
            mutations.Add(_evo);
            for (int i = 0; i < _nChildrenPerGeneration; i++)
            {
                mutations.Add(_evo.Mutate());
            }

            int count = 0;
            while (_absoluteEvolutions.Count() < 100)
            {
                Evolve(ref mutations);
                count++;
                if (OnUpdateStatus != null && _nRefreshCounter == count)
                {
                    ProgressEventArgs args = new ProgressEventArgs(Data, Fitness);
                    OnUpdateStatus(this, args);
                    count = 0;
                    //break;
                }
            }
        }

        void Evolve(ref List<IEvolution> mutants)
        {
            List<IEvolution> mutations = new List<IEvolution>();
            foreach (IEvolution mutant in mutants)
            {
                List<IEvolution> tempMutations = new List<IEvolution>();
                for (int i = 0; i < _nMutations; i++)
                {
                    tempMutations.Add(mutant.Mutate());
                }
                tempMutations.Add(mutant);
                tempMutations.Sort(_evoComparer);
                for(int j=1;j< _nMutations; j++)
                {
                    if(tempMutations[0] == tempMutations[j])
                    {
                        mutations.Add(tempMutations[j]);
                    }
                    else
                    {
                        break;
                    }
                }
                mutations.Add(tempMutations[0]);
            }

            mutations.Sort(_evoComparer);

            //keep only the best fit
            mutants.Clear();
            Data = mutations[0].Data;
            Fitness = mutations[0].Fitness;
            for (int i = 0; i < _nChildrenPerGeneration; i++)
            {

                if (mutations[i].Fitness == mutations[i].DesiredFitness)
                {
                    mutations[i].Serialize();
                    _absoluteEvolutions.Add(mutations[i]);
                }
                else
                {
                    mutants.Add(mutations[i]);
                }
            }

            //introduce some free mutations
            for (int i = 0; i < _nFreshMutationsPerGeneration; i++)
            {
                mutants.Add(_evo.Mutate());
            }

            
        }

        public object Data { get; private set; }
        public int Fitness { get; private set; }
    }

    public class ProgressEventArgs : EventArgs
    {
        public object Data{ get; set; }
        public int Fitness { get; set; }

        public ProgressEventArgs(object data, int fitness)
        {
            Data = data;
            Fitness = fitness;
        }
    }
}
