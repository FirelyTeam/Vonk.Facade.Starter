using Hl7.Fhir.Model;
using Vonk.Core.Common;
using Vonk.Facade.Starter.Models;
using Vonk.Fhir.R3;

namespace Vonk.Facade.Starter.Repository
{
    public class ResourceMapper
    {
        public IResource MapPatient(ViSiPatient source)
        {
            var patient = new Patient
            {
                Id = source.Id.ToString(),
                BirthDate = new FhirDateTime(source.DateOfBirth).ToString()
            };
            patient.Identifier.Add(new Identifier("http://mycompany.org/patientnumber", source.PatientNumber));
            patient.Name.Add(new HumanName().WithGiven(source.FirstName).AndFamily(source.FamilyName));
            patient.Telecom.Add(new ContactPoint(ContactPoint.ContactPointSystem.Email, ContactPoint.ContactPointUse.Home, source.EmailAddress));
            return patient.AsIResource();
        }
    }
}
