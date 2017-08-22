using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Tests
{
  [TestClass]
  public class StudentTest : IDisposable
  {
    public StudentTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=university_registrar_test;";
    }
    public void Dispose()
    {
      Course.DeleteAll();
      Student.DeleteAll();
    }

    [TestMethod]
    public void GetAll_DatebaseEmptyAtFirst_0()
    {
      //Arrange, Act
      int result = Student.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Equals_TrueForSameDescription_Student()
    {
      //Arrange, Act
      Student firstStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
      Student secondStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));

      bool result = firstStudent.Equals(secondStudent);

      //Assert
      Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Save_StudentSavesToDatebase_StudentList()
    {
      //Arrange
      Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
      testStudent.Save();

      //Act
      List<Student> result = Student.GetAll();
      List<Student> testList = new List<Student>{testStudent};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Save_AssignsIdToObject_id()
    {
      //Arrange
      Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
      testStudent.Save();

      //Act
      Student savedStudent = Student.GetAll()[0];

      int result = savedStudent.GetId();
      int testId = testStudent.GetId();

      //Assert
      Assert.AreEqual(testId, result);
    }

    [TestMethod]
    public void Find_FindsStudentInDatebase_Student()
    {
      //Arrange
      Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
      testStudent.Save();

      //Act
      Student result = Student.Find(testStudent.GetId());

      //Assert
      Assert.AreEqual(testStudent, result);
    }
  [TestMethod]
  public void AddCourse_AddsCourseToStudent_CourseList()
  {
    //Arrange
    Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
    testStudent.Save();

    Course testCourse = new Course("Philosophy", "PHIL103");
    testCourse.Save();


    //Act
    testStudent.AddCourseToJoinTable(testCourse);

    List<Course> result = testStudent.GetCourses();
    List<Course> testList = new List<Course>{testCourse};

    //Assert
    CollectionAssert.AreEqual(testList, result);
  }

  [TestMethod]
  public void GetCourses_ReturnsAllCoursesFromStudent_CoursesList()
  {
    //Arrange
    Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
    testStudent.Save();

    Course testCourse = new Course("Philosophy", "PHIL103");
    testCourse.Save();

    Course testCourse2 = new Course("History", "HIST204");
    testCourse2.Save();

    //Act
    testStudent.AddCourseToJoinTable(testCourse2);
    List<Course> result = testStudent.GetCourses();
    List<Course> testList = new List<Course> {testCourse2};

    //Assert
    CollectionAssert.AreEqual(testList, result);
  }
    [TestMethod]
    public void Delete_DeletesStudentAssociationsFromDatebase_StudentList()
    {
      //Arrange
      Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
      testStudent.Save();

      Course testCourse = new Course("Philosophy", "PHIL103");
      testCourse.Save();

      //Act
      testStudent.AddCourseToJoinTable(testCourse);
      testStudent.Delete();

      List<Student> resultStudentList= testCourse.GetStudents();
      List<Student> testStudentList = new List<Student> {};

      //Assert
      CollectionAssert.AreEqual(testStudentList, resultStudentList);
    }
  }
}
