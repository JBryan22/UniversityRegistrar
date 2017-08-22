
using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;
using System.Collections.Generic;
using System;

namespace UniversityRegistrar.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet("/course-list")]
    public ActionResult CourseList()
    {
      return View(Course.GetAll());
    }

    [HttpGet("/student-list")]
    public ActionResult StudentList()
    {
      return View(Student.GetAll());
    }

    [HttpGet("/course/{id}")]
    public ActionResult CourseDetails(int id)
    {
      var selectedCourse = Course.Find(id);
      var model = new Dictionary<string, object>();
      model.Add("courses", selectedCourse);
      model.Add("in-course", selectedCourse.GetStudents());
      model.Add("out-of-course", selectedCourse.GetStudents(false));

      return View(model);
    }

    [HttpGet("/student/{id}")]
    public ActionResult StudentDetails(int id)
    {
      var model = new Dictionary<string, object>();
      model.Add("students", Student.Find(id));
      model.Add("courses", Student.Find(id).GetCourses());

      return View(model);
    }

    [HttpPost("/course-list")]
    public ActionResult CourseNew()
    {

      string name = Request.Form["course-name"];
      string courseNumber = Request.Form["course-number"];

      var newCourse = new Course(name, courseNumber);
      newCourse.Save();


      return View("CourseList", Course.GetAll());
    }

    [HttpPost("/student-list")]
    public ActionResult StudentNew()
    {
      string name = Request.Form["student-name"];
      DateTime enrollment = Convert.ToDateTime(Request.Form["student-number"]);

      var newStudent = new Student(name, enrollment);
      newStudent.Save();

      return View("StudentList", Student.GetAll());
    }

    [HttpPost("/course/{id}")]
    public ActionResult CourseDetailsNew(int id)
    {
      string studentId = Request.Form["add-student"];
      var studentIdInt = Int32.Parse(studentId);
      var selectedCourse = Course.Find(id);
      var newStudent = Student.Find(studentIdInt );
      newStudent.AddCourseToJoinTable(selectedCourse);


      var model = new Dictionary<string, object>();
      model.Add("courses", selectedCourse);
      model.Add("in-course", selectedCourse.GetStudents());
      model.Add("out-of-course", selectedCourse.GetStudents(false));

      return View("CourseDetails", model);
    }

    [HttpGet("/delete")]
    public ActionResult DeleteStuff()
    {
      Student.DeleteAll();
      Course.DeleteAll();

      return View("Index");


    }

  }
}
