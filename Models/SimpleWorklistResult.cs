namespace SimpleDICOMToolkit.Models
{
    public class SimpleWorklistResult
    {
        public string Name { get; private set; }

        public string Sex { get; private set; }

        public string Age { get; private set; }

        public string PatientId { get; private set; }

        public SimpleWorklistResult(
            string name,
            string sex,
            string age,
            string patId)
        {
            Name = name;
            Sex = sex;
            Age = age;
            PatientId = patId;
        }
    }
}
