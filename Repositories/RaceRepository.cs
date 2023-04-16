using Formula1.Models.Contracts;
using Formula1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Formula1.Repositories
{
    internal class RaceRepository : IRepository<IRace>
    {
        private List<IRace> races;
        public RaceRepository()
        {
            this.races=new List<IRace>();
        }
        public IReadOnlyCollection<IRace> Models
        {
            get { return this.races; }
        }

        public void Add(IRace model)
        {
            races.Add(model);
        }

        public IRace FindByName(string name)
        {
            return races.Find(x=>x.RaceName==name);
        }

        public bool Remove(IRace model)
        {
            return races.Remove(model);
        }
    }
}
