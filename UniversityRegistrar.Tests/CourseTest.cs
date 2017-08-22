using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Tests
{
  [TestClass]
  public class CourseTest : IDisposable
  {
    public CourseTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=university_registrar_test;";
    }

    public void Dispose()
    {
      Course.DeleteAll();
      Student.DeleteAll();
    }

    [TestMethod]
    public void GetAll_DatabaseEmptyAtFirst_0()
    {
      //Arrange, Act
      int result = Course.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Equals_TrueForSameDescription_Course()
    {
      //Arrange, Act
      Course firstCourse = new Course("Philosophy", "PHIL103");
      Course secondCourse = new Course("Philosophy", "PHIL103");

      bool result = firstCourse.Equals(secondCourse);

      //Assert
      Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void Save_CourseSavesToDatabase_CourseList()
    {
      //Arrange
      Course testCourse = new Course("Philosophy", "PHIL103");
      testCourse.Save();

      //Act
      List<Course> result = Course.GetAll();
      List<Course> testList = new List<Course>{testCourse};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Save_AssignsIdToObject_id()
    {
      //Arrange
      Course testCourse = new Course("Philosophy", "PHIL103");
      testCourse.Save();

      //Act
      Course savedCourse = Course.GetAll()[0];

      int result = savedCourse.GetId();
      int testId = testCourse.GetId();

      //Assert
      Assert.AreEqual(testId, result);
    }

    [TestMethod]
    public void Find_FindsCourseInDatabase_Course()
    {
      //Arrange
      Course testCourse = new Course("Philosophy", "PHIL103");
      testCourse.Save();

      //Act
      Course result = Course.Find(testCourse.GetId());

      //Assert
      Assert.AreEqual(testCourse, result);
    }
  [TestMethod]
  public void AddStudent_AddsStudentToCourse_StudentList()
  {
    //Arrange
    Course testCourse = new Course("Philosophy", "PHIL103");
    testCourse.Save();

    Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
    testStudent.Save();

    //Act
    testCourse.AddStudentToJoinTable(testStudent);

    List<Student> result = testCourse.GetStudents();
    List<Student> testList = new List<Student>{testStudent};

    //Assert
    CollectionAssert.AreEqual(testList, result);
  }

  [TestMethod]
  public void GetStudents_ReturnsAllCourseStudents_StudentList()
  {
    //Arrange
    Course testCourse = new Course("Philosophy", "PHIL103");
    testCourse.Save();

    Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
    testStudent.Save();

    Student testStudent2 = new Student("Charlie", new DateTime(2017, 05, 14, 11, 30, 0));
    testStudent2.Save();

    //Act
    testCourse.AddStudentToJoinTable(testStudent2);
    List<Student> result = testCourse.GetStudents();
    List<Student> testList = new List<Student> {testStudent2};

    //Assert
    CollectionAssert.AreEqual(testList, result);
  }
    [TestMethod]
    public void Delete_DeletesCourseAssociationsFromDatabase_CourseList()
    {
      //Arrange
      Student testStudent = new Student("Jesse", new DateTime(2017, 08, 15, 9, 30, 0));
      testStudent.Save();

      Course testCourse = new Course ("Philosophy", "PHIL103");
      testCourse.Save();

      //Act
      testCourse.AddStudentToJoinTable(testStudent);
      testCourse.Delete();

      List<Course> resultStudentCourses = testStudent.GetCourses();
      List<Course> testStudentCourses = new List<Course> {};

      //Assert
      CollectionAssert.AreEqual(testStudentCourses, resultStudentCourses);
    }

  }
}
