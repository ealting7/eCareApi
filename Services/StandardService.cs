using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class StandardService : IStandard
    {
        private readonly IcmsContext _icmsContext;

        public StandardService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }

        public IEnumerable<State> GetStates()
        {
            IEnumerable<State> states = Enumerable.Empty<State>();

            states = _icmsContext.States
                        .OrderBy(state => state.state_name);

            return states;
        }

        public IEnumerable<Tpas> GetTpas()
        {
            IEnumerable<Tpas> tpa = Enumerable.Empty<Tpas>();

            tpa = _icmsContext.Tpas
                        .OrderBy(t => t.tpa_name);

            return tpa;
        }

        public Tpas GetTpa(int id)
        {
            Tpas returnTpa = new Tpas();

            returnTpa = (from tpa in _icmsContext.Tpas
                         where tpa.tpa_id.Equals(id)
                         select tpa
                         ).FirstOrDefault();

            return returnTpa;
        }
    }
}
