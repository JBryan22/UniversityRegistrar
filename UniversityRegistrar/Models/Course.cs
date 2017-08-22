using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace UniversityRegistrar.Models
{
  public class  Course
  {
    private int _id;
    private string _name;
    private string _courseNumber;

    public Course(string name, string courseNumber, int id = 0)
    {
      _name = name;
      _courseNumber = courseNumber;
      _id = id;
    }

    public int GetId()
    {
      return _id;
    }

    public string GetName()
    {
      return _name;
    }

    public string GetCourseNumber()
    {
      return _courseNumber;
    }

    public override bool Equals(Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;

        bool idEquality = (this.GetId() == newCourse.GetId());
        bool nameEquality = (this.GetName() == newCourse.GetName());
        bool courseNumberEquality = (this.GetCourseNumber() == newCourse.GetCourseNumber());

        return (idEquality && nameEquality && courseNumberEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Course> GetAll()
    {
     List<Course> courseList = new List<Course> {};

     MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"SELECT * FROM courses;";

     var rdr = cmd.ExecuteReader() as MySqlDataReader;
     while(rdr.Read())
     {
       int courseId = rdr.GetInt32(0);
       string name = rdr.GetString(1);
       string courseNumber = rdr.GetString(2);
       Course newCourse = new Course(name, courseNumber, courseId);
       courseList.Add(newCourse);
     }
     conn.Close();
     return courseList;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO courses (name, course_number) VALUES (@name, @courseNumber);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter courseNumber = new MySqlParameter();
      courseNumber.ParameterName = "@courseNumber";
      courseNumber.Value = this._courseNumber;
      cmd.Parameters.Add(courseNumber);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
    }

    public static Course Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM courses WHERE id = @courseId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@courseId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int courseId = 0;
      string courseName = "";
      string courseNumber = "";

      while(rdr.Read())
      {
        courseId = rdr.GetInt32(0);
        courseName = rdr.GetString(1);
        courseNumber = rdr.GetString(2);
      }
      Course foundCourse = new Course(courseName, courseNumber, courseId);
      conn.Close();
      return foundCourse;
    }

    public void Update(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE courses SET name = @newName WHERE id = @thisId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@thisId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);

      cmd.ExecuteNonQuery();
      conn.Close();
      _name = newName;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM courses WHERE id = @thisId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@thisId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);

      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM courses;";
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public void AddStudentToJoinTable(Student newCourse)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students_courses (student_id, course_id) VALUES (@StudentId, @CourseId);";

      MySqlParameter student_id_param = new MySqlParameter();
      student_id_param.ParameterName = "@StudentId";
      student_id_param.Value = newCourse.GetId();
      cmd.Parameters.Add(student_id_param);

      MySqlParameter course_id_param = new MySqlParameter();
      course_id_param.ParameterName = "@CourseId";
      course_id_param.Value = _id;
      cmd.Parameters.Add(course_id_param);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Student> GetStudents(bool inCourse = true)
    {

        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        if (inCourse)
        {
          cmd.CommandText = @"SELECT students.* FROM courses
            JOIN students_courses ON (courses.id = students_courses.course_id)
            JOIN students ON (students.id = students_courses.student_id)
            WHERE courses.id = @CourseId;";
        }
        else
        {
          cmd.CommandText = @"SELECT students.* FROM students LEFT OUTER JOIN
          courses ON (students.id = courses.id)
          WHERE
          courses.id IS NULL
            UNION
            SELECT students.* FROM students LEFT OUTER JOIN
            students_courses ON (students.id = students_courses.student_id)
            WHERE
            students_courses.student_id IS NULL;";
        }


        MySqlParameter courseIdParameter = new MySqlParameter();
        courseIdParameter.ParameterName = "@CourseId";
        courseIdParameter.Value = _id;
        cmd.Parameters.Add(courseIdParameter);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        List<Student> students = new List<Student>{};

        while(rdr.Read())
        {
          int studentId = rdr.GetInt32(0);
          string studentName = rdr.GetString(1);
          DateTime enrollment = rdr.GetDateTime(2);
          Student newStudent = new Student(studentName, enrollment, studentId);
          students.Add(newStudent);
        }

        foreach (var student in students)
        {
          Console.WriteLine(student.GetName());
        }

        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return students;
    }
  }
}
