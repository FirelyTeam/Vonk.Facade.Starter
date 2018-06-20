using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vonk.Core.Common;
using Vonk.Core.Context;
using Vonk.Core.Repository;
using Vonk.Core.Support;
using Vonk.Facade.Relational;
using Visi.Repository.Models;

namespace Visi.Repository
{
    public class ViSiRepository : SearchRepository
    {
        private readonly ViSiContext _visiContext;
        private readonly ResourceMapper _resourceMapper;

        public ViSiRepository(QueryContext queryBuilderContext, ViSiContext visiContext, ResourceMapper resourceMapper) : base(queryBuilderContext)
        {
            Check.NotNull(visiContext, nameof(visiContext));
            Check.NotNull(resourceMapper, nameof(resourceMapper));
            _visiContext = visiContext;
            _resourceMapper = resourceMapper;
        }

        protected override async Task<SearchResult> Search(string resourceType, IArgumentCollection arguments, SearchOptions options)
        {
            switch (resourceType)
            {
                case "Patient":
                    return await SearchPatient(arguments, options);
                case "Observation":
                    return await SearchObservation(arguments, options);
                default:
                    throw new NotImplementedException($"ResourceType {resourceType} is not supported.");
            }
        }

        private async Task<SearchResult> SearchPatient(IArgumentCollection arguments, SearchOptions options)
        {
            var query = _queryContext.CreateQuery(new PatientQueryFactory(_visiContext), arguments, options);

            var count = await query.ExecuteCount(_visiContext);
            var patientResources = new List<IResource>();
            if (count > 0)
            {
                var visiPatients = await query.Execute(_visiContext).ToListAsync();

                foreach (var visiPatient in visiPatients)
                {
                    patientResources.Add(_resourceMapper.MapPatient(visiPatient));
                }
            }
            return new SearchResult(patientResources, query.GetPageSize(), count);
        }

        private async Task<SearchResult> SearchObservation(IArgumentCollection arguments, SearchOptions options)
        {
            var query = _queryContext.CreateQuery(new BPQueryFactory(_visiContext), arguments, options);

            var count = await query.ExecuteCount(_visiContext);
            var observationResources = new List<IResource>();
            if (count > 0)
            {
                var visiBloodPressures = await query.Execute(_visiContext).ToListAsync();

                foreach (var visiBloodPressure in visiBloodPressures)
                {
                    observationResources.Add(_resourceMapper.MapBloodPressure(visiBloodPressure));
                }
            }
            return new SearchResult(observationResources, query.GetPageSize(), count);
        }
    }
}
