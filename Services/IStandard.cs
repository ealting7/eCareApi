using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IStandard
    {
        IEnumerable<State> GetStates();

        IEnumerable<Tpas> GetTpas();

        Tpas GetTpa(int id);
    }
}
