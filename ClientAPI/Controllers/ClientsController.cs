using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientAPI.Models;
using Swashbuckle.AspNetCore.Annotations;


namespace ClientAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientContext _context;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(ClientContext context, ILogger<ClientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Clients
        [HttpGet("GetClientes")]
        [SwaggerOperation(Summary = "Listar todos os clientes")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
        {
            _logger.LogInformation("Buscando todos os clientes");
            return await _context.Clients.ToAsyncEnumerable().ToListAsync();
        }

        // GET: api/Clients/5
        [HttpGet("GetClientesID")]
        [SwaggerOperation(Summary = "Obter um cliente pelo ID")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                _logger.LogWarning($"Cliente com ID {id} não encontrado");
                return NotFound();
            }

            return client;
        }

        // POST: api/Clients
        [HttpPost("CreateCliente")]
        [SwaggerOperation(Summary = "Criar um novo cliente")]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Dados do cliente inválidos fornecidos");
                return BadRequest(ModelState);
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.ID }, client);
        }

        // PUT: api/Clients/5
        [HttpPut("UpdateClienteById")]
        [SwaggerOperation(Summary = "Atualizar um cliente existente pelo ID")]
        public async Task<IActionResult> UpdateClient(int id, Client client)
        {
            if (id != client.ID)
            {
                _logger.LogWarning($"ID do cliente incompatível na solicitação: {id} vs {client.ID}");
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    _logger.LogWarning($"Cliente com ID {id} não encontrado");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Clients/5
        [HttpDelete("DeleteClientesById")]
        [SwaggerOperation(Summary = "Remover um cliente pelo ID")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                _logger.LogWarning($"Cliente com ID {id} não encontrado");
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ID == id);
        }
    }
}
