using ClientAPI.Controllers;
using ClientAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ClientsControllerTeste
{
    public class ClientsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithClientsList()
        {
            try
            {
                // Arrange
                var options = new DbContextOptionsBuilder<ClientContext>()
                    .UseInMemoryDatabase(databaseName: "MyDatabase")
                    .Options;

                var context = new ClientContext(options);
                var mockLogger = new Mock<ILogger<ClientsController>>();

                var mockDbSet = MockDbSet(GetTestClients());
                context.Clients = mockDbSet.Object;

                var controller = new ClientsController(context, mockLogger.Object);

                // Act
                var result = await controller.GetAllClients();

                // Assert
                var model = Assert.IsAssignableFrom<IEnumerable<Client>>(result.Value);
                Assert.Equal(3, model.Count());
            }
            catch (Exception ex)
            {
                // Print the exception message and stack trace for debugging
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw; // Re-throw the exception to mark the test as failed
            }
        }


        [Theory]
        [InlineData("john@example.com", true)]
        [InlineData("gui14511@gmail.com", true)]
        public void EmailValidation(string email, bool expectedValidity)
        {
            bool isValid = IsValidEmail(email);
            Assert.Equal(expectedValidity, isValid);
        }

        [Theory]
        [InlineData("john.example", false)]
        [InlineData("john@example..com", false)]
        [InlineData("john@ example.com", false)]
        [InlineData("@example.com", false)]
        [InlineData("john@example", false)]
        [InlineData("john@example.", false)]
        public void EmailValidation_InvalidEmails(string email, bool expectedValidity)
        {
            bool isValid = IsValidEmail(email);
            Assert.Equal(expectedValidity, isValid);
        }

        [Theory]
        [InlineData("11963516246", true)]
        [InlineData("(11) 963516246", true)]
        [InlineData("(11) 96351-6246", true)]
        [InlineData("12345678a0", false)]
        public void PhoneNumberValidation(string phoneNumber, bool expectedValidity)
        {
            bool isValid = IsValidPhoneNumber(phoneNumber);
            Assert.Equal(expectedValidity, isValid);
        }

        private List<Client> GetTestClients()
        {
            return new List<Client>
            {
                new Client { ID = 1, FullName = "John Doe", PhoneNumber = "1234567890", Email = "john@example.com" },
                new Client { ID = 2, FullName = "Jane Doe", PhoneNumber = "0987654321", Email = "jane@example.com" },
                new Client { ID = 3, FullName = "James Bond", PhoneNumber = "1122334455", Email = "james@mi6.uk" }
            };
        }

        private Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }
        public bool IsValidEmail(string email)
        {
            // Dividindo o email em partes separadas pelo '@'
            string[] emailParts = email.Split('@');

            // Verificando se há duas partes após a divisão
            if (emailParts.Length != 2)
            {
                return false;
            }

            string username = emailParts[0];
            string domain = emailParts[1];

            // Verificando se o domínio possui pelo menos uma parte
            string[] domainParts = domain.Split('.');

            if (domainParts.Length < 1)
            {
                return false;
            }

            // Verificando se o username não está vazio e se cada parte do domínio tem pelo menos 2 caracteres
            foreach (string part in domainParts)
            {
                if (string.IsNullOrWhiteSpace(username) || part.Length < 2)
                {
                    return false;
                }
            }

            // Verificando se o último domínio é válido (.com.br ou .com)
            string lastDomainPart = domainParts[domainParts.Length - 1];
            if (lastDomainPart != "com.br" && lastDomainPart != "com")
            {
                return false;
            }

            // Verificando se não há espaços no meio do email
            if (email.Contains(" "))
            {
                return false;
            }

            return true;
        }


        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Remover espaços e caracteres não numéricos
            phoneNumber = Regex.Replace(phoneNumber, @"[^0-9]", "");

            // Verificar se possui 10 ou 11 dígitos
            if (phoneNumber.Length != 10 && phoneNumber.Length != 11)
            {
                return false;
            }

            // Verificar se começa com o código de país do Brasil (+55)
            if (phoneNumber.Length == 11)
            {
                return true;
            }

            // Padrão (XX) XXXXXXXXX
            if (phoneNumber.Length == 11)
            {
                string formattedNumber = $"({phoneNumber.Substring(2, 2)}) {phoneNumber.Substring(4)}";
                return Regex.IsMatch(formattedNumber, @"\(\d{2}\) \d{8}");
            }

            // Padrão (XX) XXXXX-XXXX
            if (phoneNumber.Length == 10 && Regex.IsMatch(phoneNumber, @"\d{2}\d{4}\d{4}"))
            {
                string formattedNumber = $"({phoneNumber.Substring(0, 2)}) {phoneNumber.Substring(2, 5)}-{phoneNumber.Substring(7)}";
                return Regex.IsMatch(formattedNumber, @"\(\d{2}\) \d{5}-\d{4}");
            }

            // Padrão XXXXXXXXXXX
            return phoneNumber.Length == 10 && Regex.IsMatch(phoneNumber, @"\d{10}");
        }
    }
}
