using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace UniversityRegistrar.Models
{
  public class Student
  {
    private int _id;
    private string _name;
    private DateTime _enrollment;

    public Student(string name, DateTime enrollment, int id = 0)
    {
      _name = name;
      _enrollment = enrollment;
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

    public DateTime GetEnrollment()
    {
      return _enrollment;
    }

    public override bool Equals(Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {
        Student newStudent = (Student) otherStudent;

        bool idEquality = (this.GetId() == newStudent.GetId());
        bool nameEquality = (this.GetName() == newStudent.GetName());
        bool enrollmentEquality = (this.GetEnrollment() == newStudent.GetEnrollment());

        return (idEquality && nameEquality && enrollmentEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public static List<Student> GetAll()
    {
     List<Student> studentList = new List<Student> {};

     MySqlConnection conn = DB.Connection();
     conn.Open();

     var cmd = conn.CreateCommand() as MySqlCommand;
     cmd.CommandText = @"SELECT * FROM students;";

     var rdr = cmd.ExecuteReader() as MySqlDataReader;
     while(rdr.Read())
     {
       int studentId = rdr.GetInt32(0);
       string name = rdr.GetString(1);
       DateTime enrollment = rdr.GetDateTime(2);
       Student newStudent = new Student(name, enrollment, studentId);
       studentList.Add(newStudent);
     }
     conn.Close();
     return studentList;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students (name, enrollment) VALUES (@name, @enrollment);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter enrollment = new MySqlParameter();
      enrollment.ParameterName = "@enrollment";
      enrollment.Value = this._enrollment;
      cmd.Parameters.Add(enrollment);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
    }

    public static Student Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM students WHERE id = @studentId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@studentId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int studentId = 0;
      string studentName = "";
      DateTime enrollment = DateTime.Now;

      while(rdr.Read())
      {
        studentId = rdr.GetInt32(0);
        studentName = rdr.GetString(1);
        enrollment = rdr.GetDateTime(2);
      }
      Student foundStudent = new Student(studentName, enrollment, studentId);
      conn.Close();
      return foundStudent;
    }

    public void Update(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE students SET name = @newName WHERE id = @thisId;";

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
      cmd.CommandText = @"DELETE FROM students WHERE id = @thisId;";

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
      cmd.CommandText = @"DELETE FROM students;";
      cmd.ExecuteNonQuery();
      conn.Close();
    }

    public void AddCourseToJoinTable(Course newCourse)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students_courses (student_id, course_id) VALUES (@StudentId, @CourseId);";

      MySqlParameter course_id_param = new MySqlParameter();
      course_id_param.ParameterName = "@CourseId";
      course_id_param.Value = newCourse.GetId();
      cmd.Parameters.Add(course_id_param);

      MySqlParameter student_id_param = new MySqlParameter();
      student_id_param.ParameterName = "@StudentId";
      student_id_param.Value = _id;
      cmd.Parameters.Add(student_id_param);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Course> GetCourses()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT courses.* FROM students
          JOIN students_courses ON (students.id = students_courses.student_id)
          JOIN courses ON (courses.id = students_courses.course_id)
          WHERE students.id = @StudentId;";

        MySqlParameter studentIdParameter = new MySqlParameter();
        studentIdParameter.ParameterName = "@StudentId";
        studentIdParameter.Value = _id;
        cmd.Parameters.Add(studentIdParameter);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        List<Course> courses = new List<Course>{};

        while(rdr.Read())
        {
          int courseId = rdr.GetInt32(0);
          string courseName = rdr.GetString(1);
          string courseNumber = rdr.GetString(2);
          Course newCourse = new Course(courseName, courseNumber, courseId);
          courses.Add(newCourse);
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return courses;
    }

  }
}
