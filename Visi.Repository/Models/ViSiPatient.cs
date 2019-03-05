using System;
using System.Collections.Generic;

namespace Visi.Repository.Models
{
    public partial class ViSiPatient
    {
        public ViSiPatient()
        {
            BloodPressure = new HashSet<ViSiBloodPressure>();
        }

        public int Id { get; set; }
        public string PatientNumber { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public ICollection<ViSiBloodPressure> BloodPressure { get; set; }
    }
}
