using ContactManager.Contracts.Contacts;
using ContactManager.Grpc.Contacts; // Generated from contacts.proto
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpcException = global::Grpc.Core.RpcException;
using GrpcStatusCode = global::Grpc.Core.StatusCode;


namespace ContactManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("contacts")]
public sealed class ContactsController : ControllerBase
{
    private readonly ContactsService.ContactsServiceClient _grpc;

    public ContactsController(ContactsService.ContactsServiceClient grpc)
    {
        _grpc = grpc;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactDto>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            // Deadline example: fail fast if gRPC hangs
            var reply = await _grpc.GetContactAsync(
                new GetContactRequest { Id = id.ToString() },
                deadline: DateTime.UtcNow.AddMilliseconds(300),
                cancellationToken: ct);

            return Ok(new ContactDto(
                Guid.Parse(reply.Id),
                reply.Name,
                string.IsNullOrWhiteSpace(reply.Email) ? null : reply.Email,
                string.IsNullOrWhiteSpace(reply.Phone) ? null : reply.Phone
            ));
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.DeadlineExceeded)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout,
                new { message = "Upstream gRPC timed out." });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create([FromBody] CreateContactRequest req, CancellationToken ct)
    {
        var reply = await _grpc.UpsertContactAsync(
            new UpsertContactRequest
            {
                // empty id means create
                Name = req.Name,
                Email = req.Email ?? "",
                Phone = req.Phone ?? ""
            },
            deadline: DateTime.UtcNow.AddSeconds(2),
            cancellationToken: ct);

        var dto = new ContactDto(
            Guid.Parse(reply.Id),
            reply.Name,
            string.IsNullOrWhiteSpace(reply.Email) ? null : reply.Email,
            string.IsNullOrWhiteSpace(reply.Phone) ? null : reply.Phone);

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactDto>> Update(Guid id, [FromBody] UpdateContactRequest req, CancellationToken ct)
    {
        var reply = await _grpc.UpsertContactAsync(
            new UpsertContactRequest
            {
                Id = id.ToString(),
                Name = req.Name,
                Email = req.Email ?? "",
                Phone = req.Phone ?? ""
            },
            deadline: DateTime.UtcNow.AddSeconds(2),
            cancellationToken: ct);

        return Ok(new ContactDto(
            Guid.Parse(reply.Id),
            reply.Name,
            string.IsNullOrWhiteSpace(reply.Email) ? null : reply.Email,
            string.IsNullOrWhiteSpace(reply.Phone) ? null : reply.Phone));
    }

    [HttpGet]
    public async Task<ActionResult<List<ContactDto>>> GetAll(CancellationToken ct)
    {
        var reply = await _grpc.ListContactsAsync(
            new ListContactsRequest(),
            deadline: DateTime.UtcNow.AddSeconds(2),
            cancellationToken: ct);

        var list = reply.Contacts.Select(c => new ContactDto(
            Guid.Parse(c.Id),
            c.Name,
            string.IsNullOrWhiteSpace(c.Email) ? null : c.Email,
            string.IsNullOrWhiteSpace(c.Phone) ? null : c.Phone
        )).ToList();

        return Ok(list);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _grpc.DeleteContactAsync(
                new DeleteContactRequest { Id = id.ToString() },
                deadline: DateTime.UtcNow.AddSeconds(2),
                cancellationToken: ct);

            return NoContent();
        }
        catch (RpcException ex) when (ex.StatusCode == GrpcStatusCode.NotFound)
        {
            return NotFound();
        }
    }
}
