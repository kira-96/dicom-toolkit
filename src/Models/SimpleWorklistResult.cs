namespace SimpleDICOMToolkit.Models
{
    /// <summary>
    /// 列表显示的 Worklist 查询结果
    /// </summary>
    public class SimpleWorklistResult
    {
        public string Name { get; }

        public string Sex { get; }

        public string Age { get; }

        public string PatientId { get; }

        public string StudyUID { get; }

        public SimpleWorklistResult(
            string name,
            string sex,
            string age,
            string patId,
            string studyUid)
        {
            Name = name;
            Sex = sex;
            Age = age;
            PatientId = patId;
            StudyUID = studyUid;
        }
    }
}
