﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject_Danyal.Models;
using System.Diagnostics;

namespace PassionProject_Danyal.Controllers
{
    public class TeamDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Gets a list or Teams in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Teams including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/TeamData/GetTeams
        /// </example>
        [ResponseType(typeof(IEnumerable<TeamDto>))]
        public IHttpActionResult GetTeams()
        {
            List<Team> Teams = db.Teams.ToList();
            List<TeamDto> TeamDtos = new List<TeamDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Team in Teams)
            {
                TeamDto NewTeam = new TeamDto
                {
                    TeamID = Team.TeamID,
                    TeamName = Team.TeamName,
                    TeamColor = Team.TeamColor
                };
                TeamDtos.Add(NewTeam);
            }

            return Ok(TeamDtos);
        }


        /// <summary>
        /// Gets a list of drivers in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input teamid</param>
        /// <returns>A list of drivers associated with the team</returns>
        /// <example>
        /// GET: api/TeamData/GetDriversForTeam
        /// </example>
        [ResponseType(typeof(IEnumerable<DriverDto>))]
        public IHttpActionResult GetDriversForTeam(int id)
        {
            List<Driver> Drivers = db.Drivers.Where(p => p.TeamID == id)
                .ToList();
            List<DriverDto> DriverDtos = new List<DriverDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Driver in Drivers)
            {
                DriverDto NewDriver = new DriverDto
                {
                    DriverID = Driver.DriverID,
                    Name = Driver.Name,
                    PSNTag = Driver.PSNTag,
                    Nationality = Driver.Nationality,
                    Abbreviation = Driver.Abbreviation,
                    Status = Driver.Status,
                    TeamID = Driver.TeamID
                };
                DriverDtos.Add(NewDriver);
            }

            return Ok(DriverDtos);
        }


        /// <summary>
        /// Finds a particular Team in the database with a 200 status code. If the Team is not found, return 404.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <returns>Information about the Team, including Team id, bio, first and last name, and teamid</returns>
        // <example>
        // GET: api/TeamData/FindTeam/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(TeamDto))]
        public IHttpActionResult FindTeam(int id)
        {
            //Find the data
            Team Team = db.Teams.Find(id);
            //if not found, return 404 status code.
            if (Team == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            TeamDto TeamDto = new TeamDto
            {
                TeamID = Team.TeamID,
                TeamName = Team.TeamName,
                TeamColor = Team.TeamColor
            };


            //pass along data as 200 status code OK response
            return Ok(TeamDto);
        }

        /// <summary>
        /// Updates a Team in the database given information about the Team.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <param name="Team">A Team object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/TeamData/UpdateTeam/5
        /// FORM DATA: Team JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateTeam(int id, [FromBody] Team Team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Team.TeamID)
            {
                return BadRequest();
            }

            db.Entry(Team).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Adds a Team to the database.
        /// </summary>
        /// <param name="Team">A Team object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/TeamData/AddTeam
        ///  FORM DATA: Team JSON Object
        /// </example>
        [ResponseType(typeof(Team))]
        [HttpPost]
        public IHttpActionResult AddTeam([FromBody] Team Team)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Teams.Add(Team);
            db.SaveChanges();
            Debug.WriteLine(Team.TeamName);
            return Ok(Team.TeamID);
        }

        [HttpPost]
        public IHttpActionResult Test([FromBody]Team MyTeam)
        {
            Debug.WriteLine(MyTeam.TeamColor);
            return Ok(MyTeam.TeamName);
        }
        /// <summary>
        /// Deletes a Team in the database
        /// </summary>
        /// <param name="id">The id of the Team to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/TeamData/DeleteTeam/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteTeam(int id)
        {
            Team Team = db.Teams.Find(id);
            if (Team == null)
            {
                return NotFound();
            }

            db.Teams.Remove(Team);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a Team in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <returns>TRUE if the Team exists, false otherwise.</returns>
        private bool TeamExists(int id)
        {
            return db.Teams.Count(e => e.TeamID == id) > 0;
        }
    }
}
