﻿using Application.Students.Queries;
using Domain.Entities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using static Application.IntegrationTest.TestHelper;
using Application.Admin.Commands.StudentCommand;

namespace Application.IntegrationTest.Students.Courses
{
    public class GetCoursesQueryTest
    {
        [Test]
        public async Task GetCourses_ShouldReturnCourseOfStudentFromDataBaseTest()
        {
            await RunAsTeacherAsync();
            var teacher = await GetAuthenticatedUser<Teacher>();

            await RunAsStudentAsync();
            var student = await GetAuthenticatedUser<Student>();
            var course = CreateCourse(teacher);
            await AddAsync(course);
            var studentCourse = CreateStudentCourse(course, student);
            await AddAsync(studentCourse);

            var them = CreateTheme(course, DateTime.Now);
            await AddAsync(them);
            them = CreateTheme(course, DateTime.Now.AddDays(1));
            await AddAsync(them);
            them = CreateTheme(course, DateTime.Now.AddDays(-2));
            await AddAsync(them);

            GetCoursesQuery query = new(student.Id);

            var coursesDto = await SendAsync(query);

            coursesDto.Should().Contain(s => s.Name.Equals(course.Name));

            coursesDto.FirstOrDefault(s => s.Name == course.Name).TotalThemes.Should().Be(3);
            coursesDto.FirstOrDefault(s => s.Name == course.Name).CompletedThemes.Should().Be(1);
        }

        [Test]
        public async Task GetCompletedThemesCount_ShouldBeReturnCourseOfStudentFromDataBaseTest()
        {
            await RunAsStudentAsync();

            // Arrange
            var student = GetStudentCommand("Hasanboy", "hasanboy@mail.ru");
            await SendAsync(student);
            var hasanboy = await GetAsync<Student>(s => s.Email == student.Email);

            student = GetStudentCommand("Vosid", "vosid@mail.ru");
            await SendAsync(student);
            var vosid = await GetAsync<Student>(s => s.Email == student.Email);

            Course course = await CreateTestData(hasanboy, vosid);

            // Act
            GetCoursesQuery query = new(hasanboy.Id);
            var hasanboyDTO = await SendAsync(query);
            query = new(vosid.Id);
            var vosidDTO = await SendAsync(query);

            // Assert
            hasanboyDTO.Should().Contain(s => s.Name.Equals(course.Name));
            hasanboyDTO.FirstOrDefault(s => s.Name == course.Name).TotalThemes.Should().Be(1);
            hasanboyDTO.FirstOrDefault(s => s.Name == course.Name).CompletedThemes.Should().Be(0);

            vosidDTO.Should().Contain(s => s.Name.Equals(course.Name));
            vosidDTO.FirstOrDefault(s => s.Name == course.Name).TotalThemes.Should().Be(1);
            vosidDTO.FirstOrDefault(s => s.Name == course.Name).CompletedThemes.Should().Be(1);
        }

        #region Test Data
        private async Task<Course> CreateTestData(Student hasanboy, Student vosid)
        {
            await RunAsTeacherAsync();
            var teacher = await GetAuthenticatedUser<Teacher>();

            var course = CreateCourse(teacher);
            await AddAsync(course);

            var theme = CreateTheme(course, DateTime.Now.AddDays(-2));
            await AddAsync(theme);

            var variable = await CreateExercise("Variables", 1, theme.Id);

            var studentCourse = CreateStudentCourse(course, hasanboy);
            await AddAsync(studentCourse);

            await CreateStudentCourseExercise(studentCourse.Id, variable.Id, Status.Failed);

            studentCourse = CreateStudentCourse(course, vosid);
            await AddAsync(studentCourse);

            await CreateStudentCourseExercise(studentCourse.Id, variable.Id, Status.Passed);
            return course;
        }

        Course CreateCourse(Teacher teacher)
        {
            return new Course()
            {
                Id = Guid.NewGuid(),
                Name = "C# Training",
                LearningLanguage = "Tajik",
                TeacherId = teacher.Id
            };
        }
        Theme CreateTheme(Course course, DateTime endDate)
        {
            return new Theme()
            {
                Id = Guid.NewGuid(),
                Content = "Content",
                Name = "Name",
                CourseId = course.Id,
                StartDate = endDate.AddDays(-3),
                EndDate = endDate
            };
        }
        StudentCourse CreateStudentCourse(Course course, Student student)
        {
            return new StudentCourse()
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                StudentId = student.Id,
            };
        }
        async Task<StudentCourseExercise> CreateStudentCourseExercise(Guid studentCourseId, Guid exerciseId, Status status)
        {
            var studentCourseExercise = new StudentCourseExercise()
            {
                StudentCourseId = studentCourseId,
                ExerciseId = exerciseId,
                Status = status,
                Code = "string"
            };
            await AddAsync(studentCourseExercise);
            return studentCourseExercise;
        }

        async Task<Exercise> CreateExercise(string name, int rate, Guid themId)
        {
            var exercise = new Exercise()
            {
                Id = Guid.NewGuid(),
                ThemeId = themId,
                Rating = rate,
                Name = name,
                Description = "For test",
                Template = "Template",
                Test = "Test"
            };
            await AddAsync(exercise);
            return exercise;
        }

        CreateStudentCommand GetStudentCommand(string name, string email)
        {
            return new CreateStudentCommand()
            {
                FirstName = name,
                LastName = "Glick",
                Address = "PA, Lancaster",
                BirthDate = System.DateTime.Today,
                PhoneNumber = "992927770000",
                City = "Khujand",
                Country = "Tajikistan",
                Email = email,
                Occupation = "student",
                Password = "Pw12345@",
                Region = "Sogd",
                CourseName = "C# for beginners"
            };
        }
        #endregion
    }
}