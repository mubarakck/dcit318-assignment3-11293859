namespace Healthcare_System
{
    // 2a. Generic Repository class
    public class Repository<T>
    {
        private List<T> _items = new List<T>();

        public void Add(T item)
        {
            _items.Add(item);
        }

        public List<T> GetAll()
        {
            return _items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item != null)
            {
                return _items.Remove(item);
            }
            return false;
        }
    }

    // 2b. Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    // 2c. Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Patient ID: {PatientId}, Medication: {MedicationName}, Issued: {DateIssued.ToShortDateString()}";
        }
    }

    // 2g. HealthSystemApp class
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        // Seed initial data
        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "Abdul Rashid", 21, "Male"));
            _patientRepo.Add(new Patient(2, "Sarah Wahab", 26, "Female"));
            _patientRepo.Add(new Patient(3, "Asiedu Nketiah", 70, "Male"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(101, 1, "Vitamin C", new DateTime(2024, 11, 15)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Amoxiclav", new DateTime(2024, 12, 20)));
            _prescriptionRepo.Add(new Prescription(103, 3, "Funbact 3", new DateTime(2025, 3, 10)));
            _prescriptionRepo.Add(new Prescription(104, 2, "Codeine syrup", new DateTime(2025, 4, 5)));
            _prescriptionRepo.Add(new Prescription(105, 2, "Lydia", new DateTime(2025, 5, 12)));
        }

        // Build prescription map by grouping prescriptions by PatientId
        public void BuildPrescriptionMap()
        {
            var allPrescriptions = _prescriptionRepo.GetAll();
            _prescriptionMap = allPrescriptions
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // Print all patients
        public void PrintAllPatients()
        {
            Console.WriteLine("All Patients:");
            Console.WriteLine("-----------------");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
            Console.WriteLine();
        }

        // Print prescriptions for a specific patient
        public void PrintPrescriptionsForPatient(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                var patient = _patientRepo.GetById(p => p.Id == patientId);
                Console.WriteLine($"Prescriptions for {patient?.Name} (ID: {patientId}):");
                Console.WriteLine("-----------------");
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
            else
            {
                Console.WriteLine($"No prescriptions found for patient ID: {patientId}");
            }
            Console.WriteLine();
        }

        // 2f. Alternative method to get prescriptions by patient ID
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.ContainsKey(patientId) ? _prescriptionMap[patientId] : new List<Prescription>();
        }
    }

    // Main program
    class Program
    {
        static void Main(string[] args)
        {
            var healthSystem = new HealthSystemApp();

            // i. Seed data
            healthSystem.SeedData();

            // ii. Build prescription map
            healthSystem.BuildPrescriptionMap();

            // iii. Print all patients
            healthSystem.PrintAllPatients();

            // iv. Print prescriptions for a specific patient
            Console.WriteLine("Enter Patient ID to view prescriptions (1-3):");
            if (int.TryParse(Console.ReadLine(), out int patientId))
            {
                healthSystem.PrintPrescriptionsForPatient(patientId);
            }
            else
            {
                Console.WriteLine("Invalid input. Showing prescriptions for Patient ID 1 as example.");
                healthSystem.PrintPrescriptionsForPatient(1);
            }
        }
    }
}