using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml.Serialization;


namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            var personId = 1;
            var repo = new PersonRepo();
            var result = repo.LoadPerson(personId);
            var format = "first: {0}, last:{1}, CountOfAddresses: {2}";
            Console.WriteLine(format, result.FirstName,result.LastName,result.Addresses.Count);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// This needs to be heavily tested and general error handling can take place here.
    /// Todo: test for non happy sql results
    /// </summary>
    /// <typeparam name="TOutEntity"></typeparam>
    public class CallStoredProcWithXmlOut<TOutEntity>
    {
        public TOutEntity Execute(IDbConnection connection, IDbCommand sqlCommand, Func<string, TOutEntity> mapper)
        {
            string rawXml;
            using (connection)
            {
                connection.Open();
                sqlCommand.Connection = connection;
                var reader = sqlCommand.ExecuteReader();
                reader.Read();//just need to advance the reader 
                rawXml = (string)reader.GetValue(0);
                connection.Close();//IDisposable should catch this in the using, but here for good measure.
            }
            var mapperResult = mapper.Invoke(rawXml);
            return mapperResult;
        }
    }

    public class PersonRepo: IPersonRepo
    {
        /// <summary>
        /// Since you cannot mock the SqlCommand class, we need to use the builder and pass it in.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Person LoadPerson(IDbConnection connection, IDbCommand sqlCommand, Func<string, Person> mapper)
        {
            var call = new CallStoredProcWithXmlOut<Person>();
            var result = call.Execute(connection, sqlCommand, mapper);
            return result;
        }

        /// <summary>
        /// SqlCommand can't be mocked, so it needs to be injected in for the implementation.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public SqlCommand LoadPersonCommandBuilder(int personId)
        {
            var command = new SqlCommand()
                              {
                                  CommandType = CommandType.StoredProcedure,
                                  CommandText = "dbo.GetPersonXml"
                              };
            command.Parameters.Add(new SqlParameter("@PersonId", personId));
            return command;
        }

        public Person LoadPerson(int personId)
        {
            var connectionStr = ConfigurationManager.ConnectionStrings["Sql2008"].ConnectionString;
            var connetion = new SqlConnection(connectionStr);
            var personMapper = new PersonMapper();
            var result = LoadPerson(connetion, LoadPersonCommandBuilder(personId),personMapper.MapFromRawXmlString);
            return result;
        }
    }

  public class PersonMapper
  {
      public Person MapFromRawXmlString(string xml)
      {
          Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(xml));

          Person person;
          var ser = new XmlSerializer(typeof (Person));
          using (var reader = new StringReader(xml))
          {
              person = (Person) ser.Deserialize(reader);
          }
          return person;
      }
  }


   public class Person
   {
       public int PersonId { get; set; }
       public string FirstName { get; set; }
       public string LastName { get; set; }
       public List<Address> Addresses { get; set; }


   }
    public class Address
    {
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }

    }
    public interface IPersonRepo
    {
        Person LoadPerson(int personId);
    }
   
}
