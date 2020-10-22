using Common.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;

namespace CommonTests
{
    public class Tests
    {
        #region Public Methods

        [Test]
        public void ImportTrainPathMessages()
        {
            var content = System.IO.File.ReadAllText("TrainPathMessages.json");
            var messages = JsonConvert.DeserializeObject<TrainPathMessage[]>(content);

            Assert.True(messages.Any());
        }

        #endregion Public Methods
    }
}