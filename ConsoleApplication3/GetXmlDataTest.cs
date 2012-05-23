using System;
using System.Data;
using System.Data.SqlTypes;
using NUnit.Framework;
using Moq;

namespace ConsoleApplication3
{
 public class GetXmlDataTest
    {

     public Mock<IDbConnection> SqlConn { get; set; }

     const int PersonId = 1;
     public PersonRepo Repo { get; set; }
     public Person RepoResult { get; set; }
     public Mock<IDbCommand> SqlCommand { get; set; }
     public Mock<IDataReader> DataReader { get; set; }

     [SetUp]
     public void SetUp()
     {
         Repo = new PersonRepo();

         //const string connectionStr = @"Server=.\SQLEXPRESS;Database=XmlTest;Trusted_Connection=True;";

         SqlConn = new Mock<IDbConnection>();
         //SqlConn.Setup(x => x.ConnectionString).Returns(connectionStr);

         DataReader = new Mock<IDataReader>();
         DataReader.Setup(x => x.GetValue(0)).Returns(GoodPersonRawXml);

         SqlCommand = new Mock<IDbCommand>();
         SqlCommand.Setup(x => x.ExecuteReader()).Returns(DataReader.Object);
         //SqlCommand.Setup(x=> x.ExecuteReader()).Returns(ThrowError);
         //The commands below are not needed for tests, but do provide guidance to what is needed in an implementation.
         //SqlCommand.Setup(x => x.CommandType).Returns(CommandType.StoredProcedure);
         //SqlCommand.Setup(x => x.CommandText).Returns("dbo.GetPersonXml");

         var mapper = new PersonMapper();
         RepoResult = Repo.LoadPerson(SqlConn.Object, SqlCommand.Object, mapper.MapFromRawXmlString);
     }

     /// <summary>
     /// This is not a real world error, I just picked something
     /// </summary>
     [Test]
     [ExpectedException(typeof(System.Exception))]
     public void ExecuteReaderThrowsError()
     {
         var mapper = new PersonMapper();
         SqlCommand.Setup(x => x.ExecuteReader()).Returns(ThrowError);
         RepoResult = Repo.LoadPerson(SqlConn.Object, SqlCommand.Object, mapper.MapFromRawXmlString);
     }

     /// <summary>
     /// This argument should be thrown from the mapper.
     /// </summary>
     [Test]
     [ExpectedException(typeof(ArgumentOutOfRangeException))]
     public void ReaderReturnsEmptyString()
     {
         var mapper = new PersonMapper();
         DataReader.Setup(x => x.GetValue(0)).Returns(String.Empty);
         RepoResult = Repo.LoadPerson(SqlConn.Object, SqlCommand.Object, mapper.MapFromRawXmlString);
     }

     [Test]
     public void DataReader_Called_Read_And_GetValue0_JustOneTime()
     {
         RepoResult.FirstName = "Eric";
         DataReader.Verify(x=>x.Read(),Times.Once());
         DataReader.Verify(x=>x.GetValue(0),Times.Once());
     }

     [Test]
     public void EnsureOnlyOneSqlConectionIsMade()
     {
         SqlCommand.Verify(x => x.ExecuteReader(), Times.Once());
         SqlCommand.VerifySet(x => x.Connection, Times.Once());
     }

     [Test]
     public void SqlConnectionWasClosed()
     {
         SqlConn.Verify(x=>x.Open(),Times.Once());
         SqlConn.Verify(x=>x.Close(),Times.Once());

         //This is the important one, we don't want to keep connections open.
         //This is a big deal to test when a call fails too!
         SqlConn.Verify(x=>x.Dispose(),Times.Exactly(1));   
     }

     [Test]
     public void LoadPersonReturnsNewPerson()
     {
         Assert.AreEqual(PersonId, RepoResult.PersonId);
         Assert.AreEqual(2, RepoResult.Addresses.Count);
         Assert.AreEqual("John", RepoResult.FirstName);
         Assert.AreEqual("Doe", RepoResult.LastName);
     }

     /// <summary>
     /// No longer used, but did help get things started
     /// </summary>
     /// <param name="xml"></param>
     /// <returns></returns>
     public Person MockMapFromXml(string xml)
     {
         return new Person
         {
             PersonId = 1
         };
     }

     public IDataReader ThrowError()
     {
         //this is just a random error
         throw new Exception();
     }


     

     public string GoodPersonRawXml()
     {
         const string results = @"<Person>
              <PersonId>1</PersonId>
              <FirstName>John</FirstName>
              <LastName>Doe</LastName>
              <Addresses>
                <Address>
                  <AddressId>1</AddressId>
                  <Address1>101 Test Away</Address1>
                  <City>Norfolk</City>
                  <State>VA</State>
                </Address>
                <Address>
                  <AddressId>2</AddressId>
                  <Address1>500 Lunch Time Road</Address1>
                  <City>Virginia Beach</City>
                  <State>VA</State>
                </Address>
              </Addresses>
            </Person>";

         return results;
     }
    }
}
