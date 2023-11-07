using Moq;
using ReadingTracker.Data;

namespace ReadingTracker.Tests.Unit
{
    public class UnitTestBase
    {
        protected readonly Mock<IBookDataAccess> _mockBookDataAccess;

        public UnitTestBase() 
        {
            _mockBookDataAccess = new Mock<IBookDataAccess>();
        }
    }
}
