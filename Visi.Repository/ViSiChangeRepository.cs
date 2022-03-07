using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Visi.Repository.Models;
using Vonk.Core.Common;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Repository;
using Vonk.Core.Support;

namespace Visi.Repository
{
    [ContextAware(InformationModels = new[] { VonkConstants.Model.FhirR3 })]
    public class ViSiChangeRepository : IResourceChangeRepository
    {
        private readonly ViSiContext _visiContext;
        private readonly ResourceMapper _resourceMapper;
        public ViSiChangeRepository(ViSiContext visiContext, ResourceMapper resourceMapper)
        {
            Check.NotNull(visiContext, nameof(visiContext));
            Check.NotNull(resourceMapper, nameof(resourceMapper));
            _visiContext = visiContext;
            _resourceMapper = resourceMapper;
        }
        public async Task<IResource> Create(IResource input)
        {
            switch (input.Type)
            {
                case "Patient":
                    return await CreatePatient(input);
                case "Observation":
                    return await CreateObservation(input);
                default:
                    throw new NotImplementedException($"ResourceType {input.Type} is not supported.");
            }
        }

        private async Task<IResource> CreatePatient(IResource input)
        {
            try
            {
                var visiPatient = _resourceMapper.MapViSiPatient(input);

                var result = await _visiContext.Patient.AddAsync(visiPatient);
                await Save(visiPatient.Id.HasValue);

                // return the new resource as it was stored by this server
                return _resourceMapper.MapPatient(result.Entity);
            }
            catch (Exception ex)
            {
                throw new VonkRepositoryException($"Error on create of Patient", ex);
            }
        }

        private async Task<IResource> CreateObservation(IResource input)
        {
            try
            {
                var visiBloodPressure = _resourceMapper.MapViSiBloodPressure(input);

                var result = await _visiContext.BloodPressure.AddAsync(visiBloodPressure);
                await Save(visiBloodPressure.Id.HasValue);

                // return the new resource as it was stored by this server
                return _resourceMapper.MapBloodPressure(result.Entity);
            }
            catch (Exception ex)
            {
                throw new VonkRepositoryException($"Error on create of Observation", ex);
            }
        }
        public async Task<IResource> Delete(ResourceKey toDelete, string informationModel)
        {
            //You can ignore the informationModel. 
            //Because of the informationModel in the ContextAware attribute
            //on this class, it will only be used in a STU3 context.
            switch (toDelete.ResourceType)
            {
                case "Patient":
                    return await DeletePatient(toDelete);
                case "Observation":
                    return await DeleteObservation(toDelete);
                default:
                    throw new NotImplementedException($"ResourceType {toDelete.ResourceType} is not supported.");
            }
        }

        public async Task<IResource> DeletePatient(ResourceKey toDelete)
        {
            int toDelete_id = int.Parse(toDelete.ResourceId);
            var visiPatient = _visiContext.Patient.Find(toDelete_id);

            var bpEntries = _visiContext.BloodPressure.Where(bp => bp.PatientId == toDelete_id);
            if (!bpEntries.IsNullOrEmpty())
                throw new VonkRepositoryException($"Error on deleting Patient with Id {toDelete_id}: the patient has related Observations");

            try
            {
                var result = _visiContext.Patient.Remove(visiPatient);
                await _visiContext.SaveChangesAsync();
                return _resourceMapper.MapPatient(result.Entity);
            }
            catch (Exception ex)
            {
                throw new VonkRepositoryException($"Error on deleting Patient with Id {toDelete_id}", ex);
            }

        }

        public async Task<IResource> DeleteObservation(ResourceKey toDelete)
        {
            int toDelete_id = int.Parse(toDelete.ResourceId);
            var visiBloodPressure = _visiContext.BloodPressure.Find(toDelete_id);
            if (visiBloodPressure != null)
                return null;

            try
            {
                var result = _visiContext.BloodPressure.Remove(visiBloodPressure);
                await _visiContext.SaveChangesAsync();

                return _resourceMapper.MapBloodPressure(result.Entity);
            }
            catch (Exception ex)
            {
                throw new VonkRepositoryException($"Error on deleting Observation with Id {toDelete_id}", ex);
            }
        }

        /// <summary>
        /// This method has to return a value if you want to use it with transactions.
        /// Transactions can have circular references between resources to be created.
        /// Therefore Firely Server first needs to retrieve the ids that will be assigned to them
        /// in order to make the references correct.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public string NewId(string resourceType)
        {
            return null;
        }

        public string NewVersion(string resourceType, string resourceId)
        {
            return null;
        }

        public async Task<IResource> Update(ResourceKey original, IResource update)
        {
            switch (update.Type)
            {
                case "Patient":
                    return await UpdatePatient(original, update);
                case "Observation":
                    return await UpdateObservation(original, update);
                default:
                    throw new NotImplementedException($"ResourceType {update.Type} is not supported.");
            }
        }
        public async Task<IResource> UpdatePatient(ResourceKey original, IResource update)
        {
            try
            {
                var visiPatient = _resourceMapper.MapViSiPatient(update);

                var result = _visiContext.Patient.Update(visiPatient);
                await Save();

                return _resourceMapper.MapPatient(result.Entity);
            }
            catch (Exception ex)
            {
                throw new VonkRepositoryException($"Error on update of {original} to {update.Key()}", ex);
            }
        }

        public async Task<IResource> UpdateObservation(ResourceKey original, IResource update)
        {
            try
            {
                var visiBloodPressure = _resourceMapper.MapViSiBloodPressure(update);

                var result = _visiContext.BloodPressure.Update(visiBloodPressure);
                await Save();

                return _resourceMapper.MapBloodPressure(result.Entity);
            }
            catch (Exception ex)
            {
                throw new VonkRepositoryException($"Error on update of {original} to {update.Key()}", ex);
            }
        }

        private async Task Save(bool hasId = true)
        {
            if (!hasId) //No Id is assigned yet, let the database create one.
            {
                await _visiContext.SaveChangesAsync();
            }
            else
            {
                using (var transaction = await _visiContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //It can happen that in the database you target, an IDENTITY column is used. You can use a provided id
                        //by setting IDENTITY_INSERT to ON temporarily.
                        await _visiContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Patient ON");
                        await _visiContext.SaveChangesAsync();
                        await _visiContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Patient OFF");
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

    }
}
