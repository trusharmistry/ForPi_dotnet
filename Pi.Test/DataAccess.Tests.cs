using NUnit.Framework;

namespace Pi.Test
{
    public class DataAccessTests
    {
        [Test]
        public void Insert_TestInsert_VerifyCount()
        {
            Assert.AreEqual(expected: 1, actual: DataAccess.Insert());
        }
        
        [Test]
        public void Update_VerifyCount()
        {
            Assert.AreEqual(expected: 1, actual: DataAccess.Insert());
            Assert.AreEqual(expected: 1, actual: DataAccess.Update(DataAccess.GetLastRecordId()));
        }
    }
}