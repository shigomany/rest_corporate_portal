using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restcorporate_portal.Exceptions;
using restcorporate_portal.Models;
using restcorporate_portal.RequestModels;
using restcorporate_portal.ResponseModels;
using Swashbuckle.AspNetCore.Annotations;

namespace restcorporate_portal.Controllers
{
    static class BadgeErrorsMessages
    {
        public const string BadgeNotFound = "BADGE_NOT_FOUND";
        //public const string FileNotFoundOrUploadEmptyFile = "FILE_NOT_FOUND_OR_UPLOAD_EMPTY_FILE";
        //public const string QueryParametrsMustBeNotNull = "QUERY_PARAMETRS_MUST_BE_NOT_NULL";
    }
    [Route("api/badges")]
    [ApiController]
    public class BadgeController : ControllerBase
    {
        private readonly corporateContext _context;

        public BadgeController(corporateContext context)
        {
            _context = context;
        }

        // GET: api/badges
        [SwaggerOperation(
            Summary = "Возращает список наград",
            Tags = new string []{ "Награды" }
        )]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Ошибка", type: typeof(ExceptionInfo))]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешно", type: typeof(List<ResponseBadgeList>))]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseBadgeList>>> GetBadges()
        {
            var badges = await _context.Badges.ToListAsync();
            return Ok(badges.Select(x => ResponseBadgeList.FromApiBadge(x)).ToList());
        }

        // GET: api/badges/5
        [SwaggerOperation(
            Summary = "Детальная информация награды по id",
            Tags = new string[] { "Награды" }
        )]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBadgeList>> GetBadge(int id)
        {
            var badge = await _context.Badges.FindAsync(id);

            if (badge == null)
            {
                return NotFound(new ExceptionInfo {
                    Message = BadgeErrorsMessages.BadgeNotFound,
                    Description = "Награда не найдена"
                });
            }

            return Ok(ResponseBadgeList.FromApiBadge(badge));
        }

        // GET: api/badges
        [SwaggerOperation(
            Summary = "Возращает список моих наград",
            Tags = new string[] { "Награды" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешно", type: typeof(List<ResponseBadgeList>))]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("me/")]
        public async Task<ActionResult<IEnumerable<ResponseBadgeList>>> GetMyBadges()
        {
            var email = User.Identity.Name;
            var worker = await _context.Workers
                .Include(x => x.BadgesWorkers)
                    .ThenInclude(x => x.Badge)
                .SingleAsync(x => x.Email == email);
            return Ok(worker.BadgesWorkers.Select(x => ResponseBadgeList.FromApiBadge(x.Badge)).ToList());
        }

        //// PUT: api/Badge/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[SwaggerOperation(
        //    Summary = "",
        //    Tags = new string[] { "Награды" }
        //)]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBadge(int id, Badge badge)
        //{
        //    if (id != badge.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(badge).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BadgeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/badges/reward/
        [SwaggerOperation(
            Summary = "Наградить сотрудника достижением",
            Tags = new string[] { "Награды" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Успешно")]
        [HttpPost("reward/")]
        public async Task<IActionResult> PostBadge(RequestBadgePost requestBadgePost)
        {
            _context.BadgesWorkers.Add(new BadgesWorker {
                WorkerId = requestBadgePost.WorkerId,
                BadgeId = requestBadgePost.BadgeId
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        //// DELETE: api/Badge/5
        //[SwaggerOperation(
        //    Summary = "",
        //    Tags = new string[] { "Награды" }
        //)]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteBadge(int id)
        //{
        //    var badge = await _context.Badges.FindAsync(id);
        //    if (badge == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Badges.Remove(badge);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool BadgeExists(int id)
        //{
        //    return _context.Badges.Any(e => e.Id == id);
        //}
    }
}
