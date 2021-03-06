﻿using System.Linq;
using System.Threading;
using Features.Clients;
using MediatR;
using Moq;
using Xunit;

namespace Features.Tests
{
    [Collection(nameof(ClienteBogusCollection))]
    public class ClienteServiceTests
    {
        readonly ClienteTestsBogusFixture _clienteTestsBogus;

        public ClienteServiceTests(ClienteTestsBogusFixture clienteTestsFixture)
        {
            _clienteTestsBogus = clienteTestsFixture;
        }

        [Fact(DisplayName = "Adicionar Cliente com Sucesso")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_Adicionar_DeveExecutarComSucesso()
        {
            // Arrange
            var cliente = _clienteTestsBogus.GerarClienteValido();
            var clienteRepo = new Mock<IClientRepository>();
            var mediatr = new Mock<IMediator>();

            var clienteService = new ClientService(clienteRepo.Object, mediatr.Object);

            // Act
            clienteService.Add(cliente);

            // Assert
            Assert.True(cliente.IsValid());
            clienteRepo.Verify(r => r.Add(cliente),Times.Once);
            mediatr.Verify(m=>m.Publish(It.IsAny<INotification>(),CancellationToken.None),Times.Once);
        }

        [Fact(DisplayName = "Adicionar Cliente com Falha")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_Adicionar_DeveFalharDevidoClienteInvalido()
        {
            // Arrange
            var cliente = _clienteTestsBogus.GerarClienteInvalido();
            var clienteRepo = new Mock<IClientRepository>();
            var mediatr = new Mock<IMediator>();

            var clienteService = new ClientService(clienteRepo.Object, mediatr.Object);

            // Act
            clienteService.Add(cliente);

            // Assert
            Assert.False(cliente.IsValid());
            clienteRepo.Verify(r => r.Add(cliente), Times.Never);
            mediatr.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact(DisplayName = "Obter Clientes Ativos")]
        [Trait("Categoria", "Cliente Service Mock Tests")]
        public void ClienteService_ObterTodosAtivos_DeveRetornarApenasClientesAtivos()
        {
            // Arrange
            var clienteRepo = new Mock<IClientRepository>();
            var mediatr = new Mock<IMediator>();

            clienteRepo.Setup(c => c.GetAll())
                .Returns(_clienteTestsBogus.ObterClientesVariados());

            var clienteService = new ClientService(clienteRepo.Object, mediatr.Object);

            // Act
            var clientes = clienteService.GetAllActives();

            // Assert 
            clienteRepo.Verify(r => r.GetAll(), Times.Once);
            Assert.True(clientes.Any());
            Assert.False(clientes.Count(c=>!c.Active) > 0);
        }
    }
}