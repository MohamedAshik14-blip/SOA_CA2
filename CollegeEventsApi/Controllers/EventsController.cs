using AutoMapper;
using CollegeEventsApi.Dtos;
using CollegeEventsApi.Models;
using CollegeEventsApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeEventsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IRepository<Event> _eventRepo;
        private readonly IMapper _mapper;

        public EventsController(IRepository<Event> eventRepo, IMapper mapper)
        {
            _eventRepo = eventRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> GetAll()
        {
            var events = await _eventRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<EventReadDto>>(events));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventReadDto>> GetById(int id)
        {
            var ev = await _eventRepo.GetByIdAsync(id);
            if (ev == null) return NotFound();

            return Ok(_mapper.Map<EventReadDto>(ev));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EventReadDto>> Create(EventCreateDto dto)
        {
            var ev = _mapper.Map<Event>(dto);
            await _eventRepo.AddAsync(ev);
            await _eventRepo.SaveChangesAsync();

            var readDto = _mapper.Map<EventReadDto>(ev);
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, readDto);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, EventUpdateDto dto)
        {
            var ev = await _eventRepo.GetByIdAsync(id);
            if (ev == null) return NotFound();

            _mapper.Map(dto, ev);
            await _eventRepo.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ev = await _eventRepo.GetByIdAsync(id);
            if (ev == null) return NotFound();

            _eventRepo.Remove(ev);
            await _eventRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}
