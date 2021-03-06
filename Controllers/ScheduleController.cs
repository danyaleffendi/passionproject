﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassionProject_Danyal.Models;
using PassionProject_Danyal.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace PassionProject_Danyal.Controllers
{
    public class ScheduleController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static ScheduleController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44333/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }

        // GET: Schedule/List
        public ActionResult List()
        {
            string url = "scheduledata/getschedule";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<ScheduleDto> SelectedRace = response.Content.ReadAsAsync<IEnumerable<ScheduleDto>>().Result;
                return View(SelectedRace);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Schedule/Details/5

        public ActionResult Details(int id)
        {
            ShowSchedule ViewModel = new ShowSchedule();
            string url = "scheduledata/findschedule/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into driver data transfer object
                ScheduleDto SelectedSchedule = response.Content.ReadAsAsync<ScheduleDto>().Result;
                ViewModel.schedule = SelectedSchedule;

                
                url = "scheduledata/findwinnerforrace/" + id;
                response = client.GetAsync(url).Result;
                DriverDto SelectedDriver = response.Content.ReadAsAsync<DriverDto>().Result;
                ViewModel.driver = SelectedDriver;
                
                return View(ViewModel);

            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Schedule/Create
        public ActionResult Create()
        {
            UpdateSchedule ViewModel = new UpdateSchedule();
            //get information about driver who can win the race
            string url = "driverdata/getdrivers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<DriverDto> PotentialDrivers = response.Content.ReadAsAsync<IEnumerable<DriverDto>>().Result;
            ViewModel.alldrivers = PotentialDrivers;

            return View(ViewModel);
        }

        // POST: Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Schedule ScheduleInfo)
        {
            Debug.WriteLine(ScheduleInfo.Circuit);
            string url = "scheduledata/addschedule";
            Debug.WriteLine(jss.Serialize(ScheduleInfo));
            HttpContent content = new StringContent(jss.Serialize(ScheduleInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int RaceID = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = RaceID });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Schedule/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateSchedule ViewModel = new UpdateSchedule();

            string url = "scheduledata/findschedule/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into driver data transfer object
                ScheduleDto SelectedSchedule = response.Content.ReadAsAsync<ScheduleDto>().Result;
                ViewModel.schedule = SelectedSchedule;

                //get information about drivers who can win this race.
                url = "driverdata/getdrivers";
                response = client.GetAsync(url).Result;
                IEnumerable<DriverDto> PotentialDrivers = response.Content.ReadAsAsync<IEnumerable<DriverDto>>().Result;
                ViewModel.alldrivers = PotentialDrivers;
                
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Schedule ScheduleInfo)
        {
            Debug.WriteLine(ScheduleInfo.Round);
            string url = "scheduledata/updateschedule/" + id;
            Debug.WriteLine(jss.Serialize(ScheduleInfo));
            HttpContent content = new StringContent(jss.Serialize(ScheduleInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Schedule/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "scheduledata/findschedule/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into player data transfer object
                ScheduleDto SelectedSchedule = response.Content.ReadAsAsync<ScheduleDto>().Result;
                return View(SelectedSchedule);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Schedule/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "scheduledata/deleteschedule/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
