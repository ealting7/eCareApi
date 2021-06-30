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
    }
}
