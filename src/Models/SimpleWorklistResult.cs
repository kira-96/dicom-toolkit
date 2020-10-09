namespace SimpleDICOMToolkit.Models
{
    /// <summary>
    /// 列表显示的 Worklist 查询结果
    /// </summary>
    public class SimpleWorklistResult
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; }

        /// <summary>
        /// Patient ID
        /// </summary>
        public string PatientId { get; }

        /// <summary>
        /// Study Instance UID
        /// 用于 MPPS 操作
        /// </summary>
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
